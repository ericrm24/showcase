#ifndef PROCESSOR_H
#define PROCESSOR_H

#include <QList>
#include <QLinkedList>
#include <QQueue>
#include <QMutex>
#include <QThread>
#include <QObject>
#include <QMutex>
#include <QWaitCondition>
#include "QBarrier.h"
#include "InstructionHandler.h"


class Processor : public QThread
{
	Q_OBJECT
	friend class FileAnalyzer;
    friend class ALU;

private:
	/**
	 * @brief The BlockStates enum contains the states that a memory block can have in a directory
	 * @details U stands for unchached, M for modified and C for cached.
	 */
	enum class DirectoryBlockStates:char{U, M, C};

    /// The state of the block, modified 'M', invalid 'I', cached 'C'
    enum class CacheBlockStates:char{M, I, C};

	struct Directory
	{
	public:
		struct DirectoryBlock
		{
            /// The number of the corresponding memory block.
			short blockNumber;
			/// Stores the state of this block out of three possible.
			DirectoryBlockStates state;
			/// Indicates on which caches this block is currently stored. True if stored in the cache corresponding with that index, false o.w.
            bool activeCaches [3];
		};
	private:
		/// Stores the Directory's information by blocks
		DirectoryBlock directoryStorage[8];
		/// Used to calculate relative block adresses based on parent's processor number
		Processor* parentProcessor;

	public:
		/**
		 * @brief Directory Creates a new directory.
		 * @param parentProcessor A pointer to the processor this Directory belongs. Is used for calculation of relative block numbers.
		 */
		Directory(Processor* parentProcessor = nullptr);
		/**
		 * @brief operator [] Gets the specified memory block from the directory.
		 * @details The relative block number is deduced from the parent processor number.
		 * @param blockNumber The block number that is required. It should be provided as an absolute block number for all the memory.
		 * @return A pointer to a Directory Block struct containing the directory information for the required block.
		 */
		DirectoryBlock* operator[](int blockNumber);
	};
	/**
     * @brief The ProcessStates enum saves the possible states for a process: Ready or finished.
	 */
    enum class ProcessStates:char{r, f};
	struct Pcb
	{
		/// The id of the process stored
		int processID;
		/// Whether the stored process is running or finished
		ProcessStates state;
		/// The program counter where the execution should be resumed
		int pc;
		/// The state of the 32 processor register for this process
		int registers [32];

		/// The name of the file that stored this process
		QString fileName;
		/// Clock cycle where the process started execution
		int startClockCycle;
		/// Clock cycle where the process finished execution
		int endClockCycle;
	public:
		Pcb(int processID = 0, int pc = 0, QString originFile = QString());
	};

    /**
     * @brief Data structure that stores all the necesary values related to the
     * Data Cache simulated in the project. It has a capacity of 4 blocks of
     * 4 words (ints)
     */
	struct DataCache
    {
    public:
        struct DataCacheBlock
        {
            /// The number of the corresponding memeory block.
            short blockNumber = -1;
            /// The current state of the block between the possible three.
            CacheBlockStates state = CacheBlockStates::I;
            /// The actual data stored in the block
            int data[4] = {0};

			/**
			 * @brief cacheBlockStatetoQString Converts a CacheBlockStati into a QString.
			 * @param state The state to be converted.
			 * @return A QString with the value of the provided state.
			 */
			QString getBlockState();
        };

        /**
         * @brief operator [] Gets the memory block from the data cache.
         * @details The caller must check if the returned block is the one needed.
         * @param blockNumber The block number that is required. It should be provided as an absolute block number for all the memory.
         * @return A pointer to a Data Cache Block struct containing the information for the required block.
         */
        DataCacheBlock* operator[](int blockNumber);

        /// Store the four blocks that all data caches have
        DataCacheBlock blocks[4];
        /// Stores the number of my parent processor
        short processorNumber;
	};

    /**
     * @brief Data structure that stores all the necesary values related to the
     * Instruction Cache simulated in the project. It has a capacity of 4 blocks of
     * 4 words (ints)
     */
	struct InstructionCache
	{
    public:
        struct InstructionCacheBlock
        {
            /// The number of the corresponding memeory block.
            short blockNumber = -1;
            /// The actual instruction stored in the block
            int instruction[4][4] = {{0}};
        };

        /**
         * @brief operator [] Gets the specified memory block from the instructions cache.
         * @details The relative block number is deduced from the parent processor number.
         * @param blockNumber The block number that is required. It should be provided as an absolute block number for all the memory.
         * @return A pointer to an Instruction Cache Block struct containing the information for the required block.
         */
        InstructionCacheBlock* operator[](int blockNumber);

        /// An array with the 4 blocks
        InstructionCacheBlock blocks[4];

        /// The number of the parent processor
        short processorNumber;
	};


private:
    /// Processors array for memory access
    QVector<Processor*>& processors;
	/// Identifies which out of the three processor this one is
	short processorNumber;
    /// Performs the instruction requested operations
    ALU alu;


private:
	/// The program counter of the process in execution
	int pc;
	/// Instruction register stores the numeric value of the instruction being executed
    int ir [4];
	/// Link register used for synchronization purposes.
	int rl;
	/// Stores the process id in execution. Is used to store in the PCB during a context switch
	int currentPid;
	/// The name of the file that's currently being executed
	QString currentFile;
	/// Stores the clock cycle when this process was loaded into the processor
	int loadingClockCycle;
	/// Stores the clock cycle where the hillillo was started
	int startingClockCycle;

	/// General purpose registers
    int registers [32];
	/// The directory that controls this processor's memory
	Directory directory;
    /// Protects the processor's directory and dataMemory
    QMutex directoryMutex;
    /// Data cache with 4 blocks
    DataCache dataCache;
    /// Protects the processor's data cache and link register
    QMutex cacheMutex;
    /// Instruction cache with 4 blocks
    InstructionCache instructionCache;
	/// 8 blocks of 4 words each for data storage
    int dataMemory[8 * 4];

	/// 16 blocks of 4 words, with each word being made of 4 integers for instruction storage
	int instructionsMemory[16*4*4]{0};


	/// Stores the PCBs in execution. The list is used for the implementation of round robin calendarization
	QQueue<Pcb> activePcbs;
	/// Once a process finishes its execution it's stored here for production of final statistics
	QQueue<Pcb> finishedPcbs;
    /// Stores the max amount to which quantum should be reset after a context switch
    int maxQuantum;
    /// Stores the quantum left for the execution of instructions of the current process
    int quantum;
    /// Used for synchronization of clock cycles
    QBarrier* barrier;


public:
	/**
	 * @brief Processor Creates a new processor for execution of RISC-V programs
	 * @param parent Is created as a QObject for it to be able to manage signal events
	 * @param processorNumber An identificator for this processor, used mainly for memory calculations.
     * @param barrier A pointer to a barrier used for clock synchronization.
     * @param maxQuantum The maximum quantum to be used after a context switch
	 */
    explicit Processor(QObject *parent, QVector<Processor*>& processors, short processorNumber = 0, QBarrier* barrier = nullptr, int maxQuantum = 0);

    /**
     * @brief getProcessorNumber Getter function for the processor number
     * @return The value of this processor's processor number
     */
    short getProcessorNumber() const;

    /**
     * @brief run Starts the execution of a processor thread. Starts the event queue handling.
     */
    void run() override;

    /**
     * @brief instructionFetch Moves the next instruction to the instruction register.
     * @details If the block is not in cache, instructionCacheMiss is called before proceeding.
     */
    void instructionFetch();

    /**
     * @brief instructionCacheMiss Moves the block containing the next instruction from memory to the instruction cache.
     */
    void instructionCacheMiss();

	/**
	 * @brief Load the given block into the memory cache (Write Back).
	 * @param blockNumber The number of the block being requested.
	 */
    void loadBlockToMemory(int blockNumber);

    /**
     * @brief contextSwitch Loads the current process' context into a new pcb block, and a new pcb into the processor.
     * @details If the process has finished its execution, it is loaded into the finishedPcbs queue, active\Pcbs othrerwise.
     * @param isFinished True if the current context has finished its execution.
     * @return The process id of the new pcb loaded. 0 if there are no more active pcbs
     */
    int contextSwitch(bool isFinished);
    /**
     * @brief getInstructionsMemoryValue returns the value of a instructions memory position
     * @param position The memory address needed
     * @return The memory position requested
     */
	int getInstructionsMemoryValue(int position);
    /**
     * @brief waitClockCycles Uses the barrier to simulate the passing of @1 clock cycles
     * @param number The number of cycles that are needed to pass.
     */
    void waitClockCycles(int number);

	/**
	 * @brief writeToLog Wrapper function for signal emssion. Avoids having to use the emit command and including the processor number.
	 * @param message What needs to be displayed on screeen.
	 */
	inline void writeToLog(QString message)
	{
		emit this->newMessage(message, this->processorNumber);
	}

	/**
	 * @brief generateStatistics Prints the final statistics on screen using the newMessage signal.
	 */
	void generateStatistics();


signals:
	/**
	 * @brief newMessage Signal for the main window to add information to this processor's log
	 * @param message The information that needs to be added.
	 * @param processorNumber The identification for this processor.
	 */
	void newMessage(QString message, int processorNumber);
	/**
	 * @brief newFileExecuting Signal for the main window to change the file shown running on this processor
	 * @param filename The name of the new file running.
	 * @param processorNumber The identification for this processor.
	 */
	void newFileExecuting(QString filename, int processorNumber);
	/**
	 * @brief finishedProcessing Signal that this processor's 'hilillos' have all finished execution.
	 */
	void finishedProcessing();

public slots:

};

#endif // PROCESSOR_H
