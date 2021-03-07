#ifndef FILEANALYZER_H
#define FILEANALYZER_H


#include <QFileInfoList>
#include <QVector>


#include "Processor.h"

class FileAnalyzer
{
private:
	/// Reference to a list of files to be added to the simulation.
    QFileInfoList& filesToAnalyze;	
	/// The processors whose instructions memory should be filled.
    QVector<Processor*>& availableProcessors;	
	///Array of integers to store the value of the current filling of each processor's instructions memory.
	int* currentPCs;

public:
	/**
	 * @brief FileAnalyzer Constructor for class. Initialyzes
	 * @param processors
	 * @param files
	 */
	FileAnalyzer(QVector<Processor *>& processors, QFileInfoList& files);
	/**
	 * @brief run Executes the file analyzer, opening all of the provided files and using them to fill the processor's instructions memory
	 */
    void run();
private:
	/**
	 * @brief addFileToProcessor Adds A single file to a processors instructions memory
	 * @param processor A pointer to the processor to be filled
	 * @param file The file to be included inside the processor's instructions memory.
	 */
    void addFileToProcessor(Processor* processor, QFileInfo& file);
};

#endif // FILEANALYZER_H
