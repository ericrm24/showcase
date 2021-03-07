#include "FileAnalyzer.h"
#include "Processor.h"

#include <QFile>
#include <QTextStream>
#include <QFileInfo>
#include <QFileInfoList>

#include <iostream>

FileAnalyzer::FileAnalyzer(QVector<Processor *>& processors, QFileInfoList &files)
    : filesToAnalyze{files}
    , availableProcessors{processors}
	, currentPCs{new int [3]()}
{

}

void FileAnalyzer::run()
{
    size_t currentProcessor = 0;
    for (QFileInfoList::iterator file = this->filesToAnalyze.begin(); file != this->filesToAnalyze.end(); ++file)
	{
		this->addFileToProcessor(this->availableProcessors[currentProcessor%3], *file);
		++currentProcessor;
    }
}

void FileAnalyzer::addFileToProcessor(Processor* processor, QFileInfo& fileInfo)
{
	//Process ids start at 1, processCounter at 384 because of the position of data memory
	processor->activePcbs.enqueue(Processor::Pcb(processor->activePcbs.count()+ 1, this->currentPCs[processor->processorNumber] + 384, fileInfo.baseName()));
	QFile* file = new QFile(fileInfo.absoluteFilePath());
	file->open(QFile::ReadOnly);
	QTextStream in(file);
	while(in.status() == in.Ok)
	{
        in >> processor->instructionsMemory[this->currentPCs[processor->processorNumber]++];
	}
    this->currentPCs[processor->processorNumber]--;
//	std::cout << "Agregando el archivo " << fileInfo.path().toStdString() << " al procesador " << processor->processorNumber << " ahora tiene " << processor->activePcbs.count() << std::endl;
}
