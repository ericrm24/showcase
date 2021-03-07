#include "Processor.h"

#include <QQueue>

#include <iostream>
#include "../Interfaz/mainwindow.h"

short Processor::getProcessorNumber() const
{
    return processorNumber;
}



void Processor::run()
{
    this->writeToLog("Número de hilillos " + QString::number(this->activePcbs.count()));
	this->contextSwitch(false);
	while(!this->activePcbs.isEmpty() || this->currentPid != 0)
    {
		this->barrier->wait();
        if(this->quantum == 0)
            this->contextSwitch(false);

        else
        {
            this->instructionFetch();
            if(this->alu.executeInstruction())
            {
                this->contextSwitch(true);
            }
            else
            {
                -- this->quantum;
            }
        }
        //this->writeToLog("Current PC =  " + QString::number(this->pc) + " op code= " + QString::number(this->ir[0]));
    }
	this->writeToLog("Finished clock cycles");
	this->barrier->wait();

	emit this->finishedProcessing();

	while(reinterpret_cast<MainWindow*>(this->parent())->getFinishedProcessors() <3)
		this->barrier->wait();

	this->writeToLog("Generando estadísticas");
	this->generateStatistics();

	this->writeToLog("Ejecución finalizada");

}

void Processor::instructionFetch()
{
    Processor::InstructionCache::InstructionCacheBlock* nextInstruction;
    int blockNumber = (this->pc - 384) / 16;
    int wordNumber = ( (this->pc - 384) % 16) / 4;
    nextInstruction = this->instructionCache[this->pc];
    if (nextInstruction->blockNumber != blockNumber)
    {
		this->writeToLog("Corrigiendo fallo de caché de instrucciones");
        // Instruction cache read miss
        waitClockCycles(16);
        this->instructionCacheMiss();
        nextInstruction = this->instructionCache[this->pc];
        if (nextInstruction->blockNumber != blockNumber)
        {
            std::cout << "Something went wrong in instructionCacheMiss" << std::endl;
        }
    }
    for (int index = 0; index < 4; ++index)
    {
        // Move the 4 instruction integers to IR
        this->ir[index] = nextInstruction->instruction[wordNumber][index];
    }
    this->pc += 4;
}

void Processor::instructionCacheMiss()
{
    int blockNumber = (this->pc - 384) / 16;
    int cachePosition = blockNumber % 4;

    // Move to cache the needed block
    for (int word = 0; word < 4; ++word)
    {
        for (int param = 0; param < 4; ++param) {
            this->instructionCache.blocks[cachePosition].instruction[word][param] = this->instructionsMemory[(blockNumber*16) + (word*4) + param];
        }
    }
    // Change block number in cache
    this->instructionCache.blocks[cachePosition].blockNumber = blockNumber;
}

int Processor::contextSwitch(bool isFinished)
{
	this->writeToLog("Haciendo un cambio de contexto");
	if(this->currentPid != 0)
	{
		Pcb pcb(this->currentPid, this->pc, this->currentFile);
		for (int currentRegister = 1; currentRegister < 32; ++currentRegister)
		{
			pcb.registers[currentRegister] = this->registers[currentRegister];
		}
		if(isFinished)
		{
			pcb.state = ProcessStates::f;
			pcb.endClockCycle = static_cast<int>(reinterpret_cast<MainWindow*>(this->parent())->getCurrentClockCycle());
			pcb.startClockCycle = (this->startingClockCycle == -1) ? this->loadingClockCycle : this->startingClockCycle;
			this->finishedPcbs.enqueue(pcb);
		}
		else
		{
			pcb.state = ProcessStates::r;
			pcb.startClockCycle = (this->startingClockCycle == -1) ? this->loadingClockCycle : this->startingClockCycle;
			this->activePcbs.enqueue(pcb);
		}

	}

	if(this->activePcbs.isEmpty())
	{
		this->currentPid = 0;
        return 0;
	}
    else
    {
		Pcb pcb = this->activePcbs.dequeue();
        this->currentPid = pcb.processID;
        this->pc = pcb.pc;
		this->currentFile = pcb.fileName;
        for (int currentRegister = 1; currentRegister < 32; ++currentRegister)
        {
            this->registers[currentRegister] = pcb.registers[currentRegister];
        }
		this->quantum = this->maxQuantum;
		emit this->newFileExecuting("Ejecutando: Hilillo " + this->currentFile, this->processorNumber);
		this->rl = -1;
		this->loadingClockCycle = static_cast<int>(reinterpret_cast<MainWindow*>(this->parent())->getCurrentClockCycle());
		this->startingClockCycle = pcb.startClockCycle;
        return this->currentPid;
	}
}

int Processor::getInstructionsMemoryValue(int position)
{
    return this->instructionsMemory[position];
}

void Processor::waitClockCycles(int number)
{
    for (int count = 0; count < number; ++count)
    {
		this->barrier->wait();
	}
}



void Processor::generateStatistics()
{
    writeToLog("\nContenido de la memoria de datos");
	QString* memoryStatus = new QString;
	for (int count = 0; count < 32; ++count)
    {
        if (count % 4 == 0)
            *memoryStatus += '\n';

        // Print the absolute position of the shared memory
        *memoryStatus += "[" + QString::number((this->processorNumber * 32 + count) * 4) + "= ";
        *memoryStatus += QString::number(this->dataMemory[count]) + "] ";
	}
	writeToLog(*memoryStatus);

    writeToLog("\nContenido de la caché de datos");
    QString* dataCacheState = new QString;
    for (int block = 0; block < 4; ++block)
    {
		*dataCacheState += "Contenido del bloque " + QString::number( this->dataCache[block]->blockNumber )+ "[" + this->dataCache[block]->getBlockState() + "]" + ": ";

        for (int data = 0; data < 4; ++data)
            *dataCacheState += QString::number(this->dataCache[block]->data[data]) + " ";
        *dataCacheState += '\n';
    }
    writeToLog(*dataCacheState);

	for(QQueue<Pcb>::ConstIterator pcbIT = this->finishedPcbs.constBegin(); pcbIT != this->finishedPcbs.constEnd(); ++pcbIT)
	{
		writeToLog("Hilillo " + pcbIT->fileName);
		writeToLog("Corrió del ciclo " + QString::number(pcbIT->startClockCycle) + " al " + QString::number(pcbIT->endClockCycle));
		writeToLog("Valor de los registros:");
		for (int count = 0; count < 32; ++count)
		{
			writeToLog("r" + QString::number(count) + "= [" + QString::number( pcbIT->registers[count]) + "]");
		}
	}
}




Processor::Processor(QObject *parent, QVector<Processor*>& processors, short processorNumber, QBarrier *barrier, int maxQuantum)
    : QThread(parent)
    , processors(processors)
    , processorNumber(processorNumber)
    , alu(this)
    , pc(384)
    , rl(0)
	, currentPid(0)
    , directory(this)
    , instructionsMemory{0}
    , maxQuantum(maxQuantum)
	, quantum(0)
	, barrier(barrier)
{
	for(int count = 0; count < 32; ++count)
	{
		this->registers[count] = 0;
		this->dataMemory[count] = 1;
	}
	for(int count =0; count < 4; ++count)
	{
		this->ir[count] = 0;
	}
}

Processor::Directory::Directory(Processor* parentProcessor)
	: parentProcessor(parentProcessor)
{
	for (short block = 0; block < 8; ++block)
	{
		this->directoryStorage[block].blockNumber = block + 8 * this->parentProcessor->processorNumber;
		this->directoryStorage[block].state = DirectoryBlockStates::U;
		for(short processor = 0; processor < 3; ++processor)
			this->directoryStorage[block].activeCaches[processor] = false;
	}
}

void Processor::loadBlockToMemory(int blockNumber)
{
    blockNumber = blockNumber % 8;
    Processor::DataCache::DataCacheBlock* block = this->dataCache[blockNumber];

    for(int word = 0; word < 4; ++word)
    {
        // Lose 16 cycles
		this->waitClockCycles(16);
        this->dataMemory[blockNumber * 4 + word] = block->data[word];
    }
}

Processor::Directory::DirectoryBlock* Processor::Directory::operator[](int blockNumber)
{
	// Identifies the local block number from an absolute address
	int address = blockNumber - 8 * this->parentProcessor->processorNumber;
	// If the address is valid returns that cache block, null otherwise
    return (address >= 0 && address < 8) ? &this->directoryStorage[address] : nullptr;
}

Processor::Pcb::Pcb(int processID, int pc, QString originFile)
	: processID(processID)
	, state(ProcessStates::r)
	, pc(pc)
	, fileName(originFile)
	, startClockCycle(-1)
	, endClockCycle(-1)
{
	for(int count = 0; count < 32; ++count)
		this->registers[count] = 0;
}

Processor::DataCache::DataCacheBlock* Processor::DataCache::operator[](int blockNumber)
{
    int blockPosition = blockNumber % 4;

    // If the number is between the available 4, return it.
    return &this->blocks[blockPosition];
}

Processor::InstructionCache::InstructionCacheBlock* Processor::InstructionCache::operator[](int address)
{
    // Obtains the block number from an absolute address
    int blockNumber = (address - 384) / 16;

    int cachePosition =  blockNumber % 4;

    // If the number is between the available 4, return it.
    return &this->blocks[cachePosition];
}

QString Processor::DataCache::DataCacheBlock::getBlockState()
{
	switch (static_cast<char>(this->state))
	{
		case static_cast<char>(CacheBlockStates::C):
			return "C";
		case static_cast<char>(CacheBlockStates::I):
			return "I";
		case static_cast<char>(CacheBlockStates::M):
			return "M";
	}
}
