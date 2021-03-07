#include "InstructionHandler.h"
#include "Processor.h"

ALU::ALU(Processor* processor)
    : processor{processor}
{

}

void ALU::writeToRegister(int destReg, int value)
{
    if (destReg != 0)
        this->processor->registers[destReg] = value;
	this->processor->writeToLog("R[" + QString::number(destReg) + "] = " + QString::number(this->processor->registers[destReg]));

}

// Asumes that cache has been locked by calling function
void ALU::victim(int victimNumber)
{
    // Obtain victim block
    Processor::DataCache::DataCacheBlock* victimBlock = this->processor->dataCache[victimNumber];
    Processor::CacheBlockStates state = victimBlock->state;
    if (victimNumber != -1 && state != Processor::CacheBlockStates::I)
    {
        int homeDirectory = victimNumber / 8;

        // Lock victim home directory
        while(!this->processor->processors[homeDirectory]->directoryMutex.try_lock())
        {
            // Release resources
            this->processor->cacheMutex.unlock();
            // End cycle
            this->processor->waitClockCycles(1);
            // Reacquire resources
            while(!this->processor->cacheMutex.try_lock())
            {
                // End cycle
                this->processor->waitClockCycles(1);
            }
        }
        Processor::Directory::DirectoryBlock* dirBlock = this->processor->processors[homeDirectory]->directory[victimNumber];
        // Lose cycle for directory query
        this->processor->waitClockCycles(1);
        // If modified move to memory
        if (state == Processor::CacheBlockStates::M){
            int lostCycles = this->processor->processorNumber == homeDirectory ? 16:32;
            this->processor->waitClockCycles(lostCycles);
            fromCacheToMemory(this->processor->processorNumber, homeDirectory, victimNumber);
            // Change states
            dirBlock->state = Processor::DirectoryBlockStates::U;
            dirBlock->activeCaches[this->processor->processorNumber] = false;
            victimBlock->state = Processor::CacheBlockStates::I;
        }
        // State == 'C'
        else{
            dirBlock->activeCaches[this->processor->processorNumber] = false;
            // Check if state should be U
            bool shared = false;
            for (int index = 0; index < 3 && !shared; ++index)
            {
                shared = dirBlock->activeCaches[index];
            }
            if(!shared)
            {
                dirBlock->state = Processor::DirectoryBlockStates::U;
            }
        }
        this->processor->processors[homeDirectory]->directoryMutex.unlock();
    }
}

void ALU::loadWord(int destReg, int address, bool lr)
{
    // Try to lock the cache, if it wasn't possible, lose a clock cycle and try again
    while(!this->processor->cacheMutex.try_lock())
    {
        // End cycle
        this->processor->waitClockCycles(1);
    }

    int blockNumber = address / 16;
    int wordNumber = (address % 16) / 4;

    // Modify rl
    if (lr)
        this->processor->rl = this->processor->registers[this->processor->ir[2]];

    // Get the block that is currently being stored in the cache
    Processor::DataCache::DataCacheBlock* dataBlock = this->processor->dataCache[blockNumber];

    // If the block being stored is not the one I want, it was a cache miss
    if (dataBlock->blockNumber != blockNumber || dataBlock->state == Processor::CacheBlockStates::I)
    {
        // Do what needs to be done to the victim block
        victim(dataBlock->blockNumber);

        int homeDirectory = blockNumber / 8;

        // Try to take the directory
        while(!this->processor->processors[homeDirectory]->directoryMutex.try_lock())
        {
            // Release resources to avoid a deadlock
            this->processor->cacheMutex.unlock();

            // End cycle
            this->processor->waitClockCycles(1);

            // Reacquire resources to try again in the next clock cycle
            while(!this->processor->cacheMutex.try_lock())
            {
                // End cycle
                this->processor->waitClockCycles(1);
            }
        }

        // Get the directory of where the block is being stored
        Processor::Directory::DirectoryBlock* dirBlock = this->processor->processors[homeDirectory]->directory[blockNumber];

        // If modified move to memory
        int lostCycles = 0;

        // Lose cycle for directory query
        this->processor->waitClockCycles(1);
        if (dirBlock->state == Processor::DirectoryBlockStates::M)
        {
            for (int index = 0; index < 3; ++index)
            {
                if (dirBlock->activeCaches[index])
                {
                    while(!this->processor->processors[index]->cacheMutex.try_lock())
                    {
                        // Release resources -> only my cache
                        this->processor->cacheMutex.unlock();
                        // End cycle
                        this->processor->waitClockCycles(1);
                        // Reacquire resources
                        while(!this->processor->cacheMutex.try_lock())
                        {
                            // End cycle
                            this->processor->waitClockCycles(1);
                        }
                    }
                    // Move to memory
                    lostCycles = index == homeDirectory ? 16:32;
                    fromCacheToMemory(index, homeDirectory, blockNumber);
                    // Change to shared
                    this->processor->processors[index]->dataCache[blockNumber]->state = Processor::CacheBlockStates::C;
                    this->processor->processors[index]->cacheMutex.unlock();
                }
            }
        }
        // Move from memory
        // If it wasn't modified lost cycles are based on my 'Move from memory'
        if (lostCycles == 0)
            lostCycles = this->processor->processorNumber == homeDirectory ? 16:32;
        this->processor->waitClockCycles(lostCycles);

        // Load the block from the memory
        fromMemoryToCache(this->processor->processorNumber, homeDirectory, blockNumber);

        // Change states
        dataBlock->state = Processor::CacheBlockStates::C;
        dirBlock->state = Processor::DirectoryBlockStates::C;

        // Write in the directory that this processor uses this block
        dirBlock->activeCaches[this->processor->processorNumber] = true;

        // Release the directory
        this->processor->processors[homeDirectory]->directoryMutex.unlock();
    }

	this->processor->writeToLog("Loaded: M[" + QString::number(address) + "] = " +  QString::number(dataBlock->data[wordNumber]));

    // At last, write the word in the register and unlock the cache
    writeToRegister(destReg, dataBlock->data[wordNumber]);
    this->processor->cacheMutex.unlock();
}

void ALU::storeWord(int address, int valueToWrite, bool sc)
{
    // Lock cache
    while(!this->processor->cacheMutex.try_lock())
        this->processor->waitClockCycles(1);

    int blockNumber = address / 16;
    int wordNumber = (address % 16) / 4;

    // Check rl (first time)
    if ( !sc || (sc && this->processor->registers[this->processor->ir[1]] == this->processor->rl) )
    {
        Processor::DataCache::DataCacheBlock* dataBlock = this->processor->dataCache[blockNumber];
        // If different, cache miss
        if (dataBlock->blockNumber != blockNumber)
            victim(dataBlock->blockNumber);
        if ( (dataBlock->state != Processor::CacheBlockStates::M) &&  ( !sc || (sc && this->processor->registers[this->processor->ir[1]] == this->processor->rl) ))
        {
            int homeDirectory = blockNumber / 8;
            // Lock directory
            while (!this->processor->processors[homeDirectory]->directoryMutex.try_lock())
            {
                // Release resources
                this->processor->cacheMutex.unlock();
                // End cycle
                this->processor->waitClockCycles(1);
                // Reacquire resources
                while(!this->processor->cacheMutex.try_lock())
                {
                    this->processor->waitClockCycles(1);
                }
            }
            Processor::Directory::DirectoryBlock* dirBlock = this->processor->processors[homeDirectory]->directory[blockNumber];
            // Shared or modified by another processor
            int lostCycles = 0;
            // Lose cycle for directory query
            this->processor->waitClockCycles(1);
            // Check if it is still atomic, now that the directory has been acquired
            if ( (dirBlock->state != Processor::DirectoryBlockStates::U) &&  ( !sc || (sc && this->processor->registers[this->processor->ir[1]] == this->processor->rl) ))
            {
                // Invalidate active caches (and rl if necessary) -> move to Memory could be included here
                lostCycles = invalidateCaches(homeDirectory, blockNumber);
            }
            // Move from memory to my cache if needed
            if ( (dataBlock->blockNumber != blockNumber || dataBlock->state == Processor::CacheBlockStates::I) &&  ( !sc || (sc && this->processor->registers[this->processor->ir[1]] == this->processor->rl) ))
            {
                // If it wasn't modified
                if (lostCycles == 0)
                    lostCycles = this->processor->processorNumber == homeDirectory ? 16:32;
                this->processor->waitClockCycles(lostCycles);
                fromMemoryToCache(this->processor->processorNumber, homeDirectory, blockNumber);
            }
            // Change to modified
            if (!sc || (sc && this->processor->registers[this->processor->ir[1]] == this->processor->rl))
            {
                dataBlock->state = Processor::CacheBlockStates::M;
                dirBlock->state = Processor::DirectoryBlockStates::M;
                dirBlock->activeCaches[this->processor->processorNumber] = true;
            }
            // Release directory
            this->processor->processors[homeDirectory]->directoryMutex.unlock();
        }
        if (!sc || (sc && this->processor->registers[this->processor->ir[1]] == this->processor->rl))
        {
            // Finally, write to cache
            dataBlock->data[wordNumber] = valueToWrite;
			this->processor->writeToLog("Stored: M[" + QString::number(address) + "] = " +  QString::number(dataBlock->data[wordNumber]));
        }
        // sc true and rl different
        else
        {
            this->processor->registers[this->processor->ir[2]] = 0;
        }
    }
    // sc true and rl different
    else
    {
        this->processor->registers[this->processor->ir[2]] = 0;
    }
    this->processor->cacheMutex.unlock();
}

int ALU::invalidateCaches(int homeDirectory, int blockNumber)
{
    // Obtain directory block
    Processor::Directory::DirectoryBlock* dirBlock = this->processor->processors[homeDirectory]->directory[blockNumber];
    Processor::DataCache::DataCacheBlock* remoteCacheBlock = nullptr;
    int lostCycles = 0;
    // To invalidate can't be the same processor, must be in activeCaches
    int finished = 0;
    for (int index = 0; index < 3; ++index)
    {
        if (index == this->processor->processorNumber || !dirBlock->activeCaches[index])
            ++finished;
    }
    while (finished < 3)
    {
        // First processor
        if (0 != this->processor->processorNumber && dirBlock->activeCaches[0] && this->processor->processors[0]->cacheMutex.try_lock())
        {
            // Invalidate in next cycle
            this->processor->waitClockCycles(1);
            remoteCacheBlock = this->processor->processors[0]->dataCache[blockNumber];
            // Check RL
            if (this->processor->processors[0]->rl >= blockNumber * 16 && this->processor->processors[0]->rl < blockNumber * 16 + 16)
                this->processor->processors[0]->rl = -1;
            if (remoteCacheBlock->state == Processor::CacheBlockStates::M)
            {
                // Move to memory
                lostCycles = homeDirectory == 0 ? 16:32;
                fromCacheToMemory(0, homeDirectory, blockNumber);
            }
            // Invalidate
            dirBlock->activeCaches[0] = false;
            remoteCacheBlock->state = Processor::CacheBlockStates::I;
            this->processor->processors[0]->cacheMutex.unlock();
            ++finished;
        }
        // Second processor
        if (1 != this->processor->processorNumber && dirBlock->activeCaches[1] && this->processor->processors[1]->cacheMutex.try_lock())
        {
            // Invalidate in next cycle
            this->processor->waitClockCycles(1);
            remoteCacheBlock = this->processor->processors[1]->dataCache[blockNumber];
            // Check RL
            if (this->processor->processors[1]->rl >= blockNumber * 16 && this->processor->processors[1]->rl < blockNumber * 16 + 16)
                this->processor->processors[1]->rl = -1;
            if (remoteCacheBlock->state == Processor::CacheBlockStates::M)
            {
                // Move to memory
                lostCycles = homeDirectory == 1 ? 16:32;
                fromCacheToMemory(1, homeDirectory, blockNumber);
            }
            // Invalidate
            dirBlock->activeCaches[1] = false;
            remoteCacheBlock->state = Processor::CacheBlockStates::I;
            this->processor->processors[1]->cacheMutex.unlock();
            ++finished;
        }
        // Third processor
        if (2 != this->processor->processorNumber && dirBlock->activeCaches[2] && this->processor->processors[2]->cacheMutex.try_lock())
        {
            // Invalidate in next cycle
            this->processor->waitClockCycles(1);
            remoteCacheBlock = this->processor->processors[2]->dataCache[blockNumber];
            // Check RL
            if (this->processor->processors[2]->rl >= blockNumber * 16 && this->processor->processors[2]->rl < blockNumber * 16 + 16)
                this->processor->processors[2]->rl = -1;
            if (remoteCacheBlock->state == Processor::CacheBlockStates::M)
            {
                // Move to memory
                lostCycles = homeDirectory == 2 ? 16:32;
                fromCacheToMemory(2, homeDirectory, blockNumber);
            }
            // Invalidate
            dirBlock->activeCaches[2] = false;
            remoteCacheBlock->state = Processor::CacheBlockStates::I;
            this->processor->processors[2]->cacheMutex.unlock();
            ++finished;
        }
        // Release resources -> only my cache
        this->processor->cacheMutex.unlock();
        // End cycle
        this->processor->waitClockCycles(1);
        // Reacquire resources
        while(!this->processor->cacheMutex.try_lock())
        {
            // End cycle
            this->processor->waitClockCycles(1);
        }
    }
    // Check if state should be U
    bool shared = 0;
    for (int index = 0; index < 3 && !shared; ++index)
    {
        shared = dirBlock->activeCaches[index];
    }
    if(!shared)
    {
        dirBlock->state = Processor::DirectoryBlockStates::U;
    }
    return lostCycles;
}

void ALU::fromCacheToMemory(int cacheAt, int memoryAt, int blockNumber)
{
    Processor::DataCache::DataCacheBlock* cacheBlock = this->processor->processors[cacheAt]->dataCache[blockNumber];

    for (int word = 0; word < 4; ++word)
    {
        this->processor->processors[memoryAt]->dataMemory[(blockNumber % 8) * 4 + word] = cacheBlock->data[word];
    }
}

void ALU::fromMemoryToCache(int cacheAt, int memoryAt, int blockNumber)
{
    Processor::DataCache::DataCacheBlock* cacheBlock = this->processor->processors[cacheAt]->dataCache[blockNumber];

    for (int word = 0; word < 4; ++word)
    {
        cacheBlock->data[word] = this->processor->processors[memoryAt]->dataMemory[(blockNumber % 8) * 4 + word];
    }
	cacheBlock->blockNumber = static_cast<short>(blockNumber);
}



bool ALU::executeInstruction()
{
    // Get the OPCODE of the instruction stored in ir[0] and execute it accordingly
    switch(this->processor->ir[0])
    {
    // lw
    case 5:
    {
		this->processor->writeToLog("lw x" + QString::number(this->processor->ir[1]) +  " <- M[" +  QString::number(this->processor->ir[3]) + "+x" + QString::number(this->processor->ir[2]) + "]");
        loadWord(this->processor->ir[1], this->processor->registers[this->processor->ir[2]] + this->processor->ir[3], false);
        break;
    }

    // addi
    case 19:
    {
		this->processor->writeToLog( "addi x" + QString::number(this->processor->ir[1] ) + "<- x" + QString::number(this->processor->ir[2]) + "+" + QString::number(this->processor->ir[3] )  );
        writeToRegister(this->processor->ir[1], this->processor->registers[this->processor->ir[2]] + this->processor->ir[3]);
        break;
    }

    // sw
    case 37:
    {
		this->processor->writeToLog("sw  M[" +  QString::number(this->processor->ir[3]) + "+x" + QString::number(this->processor->ir[1]) + "]" +  " <- x" +  QString::number(this->processor->ir[2]));
        storeWord(this->processor->registers[this->processor->ir[1]] + this->processor->ir[3], this->processor->registers[this->processor->ir[2]], false);
        break;
    }

    // lr
    case 51:
    {
		this->processor->writeToLog("lr x" + QString::number(this->processor->ir[2]) +  " <- M[ x" + QString::number(this->processor->ir[1]) + "]");
        loadWord(this->processor->ir[1], this->processor->registers[this->processor->ir[2]] + this->processor->ir[3], true);
        break;
    }

    // sc
    case 52:
    {
		this->processor->writeToLog("sc  M[" +  QString::number(this->processor->ir[3]) + "+x" + QString::number(this->processor->ir[1]) + "]" +  " <- x" +  QString::number(this->processor->ir[2]));
        storeWord(this->processor->registers[this->processor->ir[1]] + this->processor->ir[3], this->processor->registers[this->processor->ir[2]], true);
        break;
    }

    // div
    case 56:
    {
		this->processor->writeToLog( "addi x" + QString::number(this->processor->ir[1] ) + "<- x" + QString::number(this->processor->ir[2]) + "/ x" + QString::number(this->processor->ir[3] )  );
        // Checks there is no division by 0			
        if (this->processor->registers[this->processor->ir[3]] != 0)		
            writeToRegister(this->processor->ir[1], this->processor->registers[this->processor->ir[2]] / this->processor->registers[this->processor->ir[3]]);
        break;
    }

    // add
    case 71:
    {
		this->processor->writeToLog( "addi x" + QString::number(this->processor->ir[1] ) + "<- x" + QString::number(this->processor->ir[2]) + "+ x" + QString::number(this->processor->ir[3] )  );
        writeToRegister(this->processor->ir[1], this->processor->registers[this->processor->ir[2]] + this->processor->registers[this->processor->ir[3]] );
        break;
    }

    // mul
    case 72:
    {
		this->processor->writeToLog( "addi x" + QString::number(this->processor->ir[1] ) + "<- x" + QString::number(this->processor->ir[2]) + "* x" + QString::number(this->processor->ir[3] )  );
        writeToRegister(this->processor->ir[1], this->processor->registers[this->processor->ir[2]] * this->processor->registers[this->processor->ir[3]] );
        break;
    }

    // sub
    case 83:
    {
		this->processor->writeToLog( "addi x" + QString::number(this->processor->ir[1] ) + "<- x" + QString::number(this->processor->ir[2]) + "- x" + QString::number(this->processor->ir[3] )  );
        writeToRegister(this->processor->ir[1], this->processor->registers[this->processor->ir[2]] - this->processor->registers[this->processor->ir[3]] );
        break;
    }

    // beq
    case 99:
    {
		this->processor->writeToLog("beq x"+  QString::number(this->processor->ir[1]) + " ==  x" + QString::number(this->processor->ir[2]) + "?" );
        if(this->processor->registers[this->processor->ir[1]] == this->processor->registers[this->processor->ir[2]])
		{
			this->processor->writeToLog("Salto condicional tomado");
            this->processor->pc+= this->processor->ir[3]*4;
		}
		else
			this->processor->writeToLog("Salto condicional ignorado");
        break;
    }

    // bne
    case 100:
    {
		this->processor->writeToLog("beq x" +  QString::number(this->processor->ir[1]) + " !=  x" + QString::number(this->processor->ir[2]) + "?" );
        if(this->processor->registers[this->processor->ir[1]] != this->processor->registers[this->processor->ir[2]])
		{
			this->processor->writeToLog("Salto condicional tomado");
            this->processor->pc+= this->processor->ir[3]*4;
		}
		else
			this->processor->writeToLog("Salto condicional ignorado");
        break;
    }

    // jalr
    case 103:
    {
		this->processor->writeToLog("jal x" + QString::number(this->processor->ir[2]) + "<- PC, PC <- x" +  QString::number(this->processor->ir[1])+ "+" + QString::number(this->processor->ir[3]) );
        writeToRegister(this->processor->ir[1], this->processor->pc);
        this->processor->pc = this->processor->registers[this->processor->ir[2]] + this->processor->ir[3];
        break;
    }

    // jal
    case 111:
    {

		this->processor->writeToLog("jal x" + QString::number(this->processor->ir[1]));
        writeToRegister(this->processor->ir[1], this->processor->pc);
        this->processor->pc += this->processor->ir[3];
        break;
    }

    case 999:
	{
		this->processor->writeToLog("FIN");
//        for (int block = 0; block < 4; ++block)
//            if (this->processor->dataCache[block]->state == Processor::CacheBlockStates::M)
//                this->fromCacheToMemory(this->processor->processorNumber, this->processor->dataCache[block]->blockNumber / 8, this->processor->dataCache[block]->blockNumber);
        return true;
    }
    default: break;
    }
    return false;
}
