#ifndef ALU_H
#define ALU_H

class Processor;

class ALU
{
  private:
    /// The processor that this ALU belongs to
    Processor* processor;



  public:
    ALU(Processor *processor);

  private:
    /**
     * @brief writeToRegister Store a value in the register indicated by the first parameter
     * @param destReg The register in which the value will be stored. Must be between 1 and 31
     * @param value The value that will be stored.
     */
    void writeToRegister(int destReg, int value );

    /**
     * @brief victim If needed, moves to memory the victim block
     * @details Handles cache coherency with the directory system
     * @details Asumes the cache has been locked by calling function.
     * @param victimNumber The specific block number of the victim.
     */
    void victim(int victimNumber);

    /**
     * @brief loadWord Moves to register the value stored in the given address, for reading
     * @details Handles cache coherency with the directory system
     * @param destReg The register to store the value. Must be between 1 and 31
     * @param address The memory address in which the value is stored
     * @param lr Boolean to indicate if it should proceed as a lr instruction
     */
    void loadWord(int destReg, int address, bool lr);

    /**
     * @brief storeWord Moves to memory the value given
     * @details Handles cache coherency with the directory system
     * @param address The memory address in which the value is stored
     * @param valueToWrite The specific value to move to memory
     * @param sc Boolean to indicate if it should proceed as a sc instruction
     */
    void storeWord(int address, int valueToWrite, bool sc);

    /**
     * @brief invalidateCaches Invalidates the caches with the block to modify
     * @details Moves to memory if the block was modified
     * @details Own cache and directory must already be acquired
     * @param homeDirectory The number of the block home directory
     * @param blockNumber The block number to invalidate
     * @return The lost cycles if the block was modified
     */
    int invalidateCaches(int homeDirectory, int blockNumber);

    /**
     * @brief fromCacheToMemory Moves to memory the block stored in cache
     * @details Needed resources must already be acquired
     * @param cacheAt The number of the processor that has the cache
     * @param memoryAt The number of the processor that has the memory block
     * @param blockNumber The block to move to memory
     */
    void fromCacheToMemory(int cacheAt, int memoryAt, int blockNumber);

    /**
     * @brief fromMemoryToCache Moves to cache the block stored in memory
     * @details Needed resources must already be acquired
     * @param cacheAt The number of the processor that has the cache
     * @param memoryAt The number of the processor that has the memory block
     * @param blockNumber The block to move to cache
     */
    void fromMemoryToCache(int cacheAt, int memoryAt, int blockNumber);

  public:
    /**
     * @brief Execute the instruction currently stored in the processor with the OPCODE given
     * by ir[0], using the parameters stored in ir[1], ir[2], ir[3].
     */
    bool executeInstruction();

};

#endif // ALU_H
