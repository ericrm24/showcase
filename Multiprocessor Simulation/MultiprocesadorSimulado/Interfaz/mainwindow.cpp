

#include <iostream>

#include <QDebug>
#include <QDir>
#include <QFileDialog>
#include <QFileInfo>
#include <QFileInfoList>
#include <QThread>
#include <QLCDNumber>
#include <QUrl>
#include <QVector>
#include <QTextDocument>

#include <assert.h>

#include "Interfaz/mainwindow.h"
#include "ui_mainwindow.h"
#include "../Modelo/Processor.h"
#include "../Modelo/FileAnalyzer.h"
#include "../Modelo/QBarrier.h"

int MainWindow::getFinishedProcessors() const
{
	return finishedProcessors;
}

size_t MainWindow::getCurrentClockCycle() const
{
	return currentClockCycle;
}

MainWindow::MainWindow(QWidget *parent)
	: QMainWindow(parent)
	, ui(new Ui::MainWindow)
	, finishedProcessors(0)
    , barrier(nullptr)
    , currentClockCycle(0)
{
    ui->setupUi(this);	
	ui->startSimulationButton->setEnabled(false);
	ui->quantumSpinBox->setMinimum(10);
    ui->clockCounterScreen->setMode(QLCDNumber::Dec);

    this->processorLogs.push_back(this->ui->plainTextEditProcesador1);
    this->processorLogs.push_back(this->ui->plainTextEditProcesador2);
	this->processorLogs.push_back(this->ui->plainTextEditProcesador3);

	this->procesorCurrentHilillo.push_back(this->ui->lineEditProcesador1);
	this->procesorCurrentHilillo.push_back(this->ui->lineEditProcesador2);
	this->procesorCurrentHilillo.push_back(this->ui->lineEditProcesador3);

}

MainWindow::~MainWindow()
{
	if(this->barrier)
		this->barrier->deleteLater();
    delete ui;
}

void MainWindow::on_openFolderButton_clicked()
{
	this->selectedFiles = this->openNewFolder();
	std::cout << "Archivos de hilillo encontrados" << std::endl;
	for (QFileInfoList::ConstIterator iterator = this->selectedFiles.constBegin(); iterator != this->selectedFiles.constEnd(); ++iterator)
	{
		std::cout << iterator->absoluteFilePath().toStdString() << std::endl;
	}
	if(!this->selectedFiles.isEmpty())
		this->ui->startSimulationButton->setEnabled(true);
	else
		this->ui->statusBar->showMessage("Elija un directorio con hilillos válidos para iniciar la simulación.", 5000);
}

void MainWindow::processorExecutionFinished()
{
	if(this->finishedProcessors == 3)
    {
		this->ui->statusBar->showMessage("Simulación finalizada");
        delete this->barrier;
		this->barrier = nullptr;
		this->processors.clear();
		this->ui->slowModeCheckBox->setEnabled(true);
		this->ui->openFolderButton->setEnabled(true);
		this->ui->quantumSpinBox->setEnabled(true);
	}
}

void MainWindow::processorFinishedProcessing()
{
	++this->finishedProcessors;
}

QFileInfoList MainWindow::openNewFolder()
{
	QDir* workingDir = new QDir(QFileDialog::getExistingDirectory(this, ("Eljia el directorio donde se encuentran los hilillos a correr"), QDir::home().absolutePath()));

	QString selectedDirMessage(workingDir->path());
	selectedDirMessage.append(" configurado como directorio para los hilillos");
	this->ui->statusBar->showMessage(selectedDirMessage, 3000);

	QStringList extensionFilter("*.txt");
	return  workingDir->entryInfoList(extensionFilter);
}

void MainWindow::on_startSimulationButton_clicked()
{
	this->ui->statusBar->showMessage("Inciando la simulación ", 3000);
	this->finishedProcessors = 0;
	this->currentClockCycle = 0;
	this->ui->slowModeCheckBox->setEnabled(false);
	this->ui->startSimulationButton->setEnabled(false);
	this->ui->openFolderButton->setEnabled(false);
	this->ui->quantumSpinBox->setEnabled(true);
	this->barrier = new QBarrier(this, this->ui->slowModeCheckBox->isChecked());

    connect(this->barrier, &QBarrier::clockCyclePassed, this, &MainWindow::updateClockCycle);
    int maxQuantum = this->ui->quantumSpinBox->value();

    for(short count = 0; count < 3; ++count)
    {
		this->processorLogs[count]->clear();
        this->processors.append(new Processor( this, this->processors, count, barrier, maxQuantum ));
    }

    FileAnalyzer fileAnalyzer(this->processors, this->selectedFiles);
    fileAnalyzer.run();
    
	for(int count = 0; count < 3; ++count)
	{
        std::cout << "Estado de la memoria de instrucciones:" << std::endl;
		QString memoryValues;
		for( int position = 0; position < 16*4*4; ++position)
		{
			memoryValues += QString::number(this->processors[count]->getInstructionsMemoryValue(position)) + " ";
		}
        std::cout << memoryValues.toStdString() << std::endl;
	}

    for(int count = 0; count < 3; ++count)
    {
		connect(processors[count], &Processor::newMessage, this, &MainWindow::addMessage);
		connect(processors[count], &Processor::finished, this, &MainWindow::processorExecutionFinished);
		connect(processors[count], &Processor::newFileExecuting, this, &MainWindow::changeFile);
		connect(processors[count], &Processor::finishedProcessing, this, &MainWindow::processorFinishedProcessing);
        this->processors[count]->start();

    }

}

void MainWindow::addMessage(QString message, int processorNumber)
{
	this->processorLogs[processorNumber]->appendPlainText(message);

}

void MainWindow::changeFile(QString message, int processorNumber)
{
	this->procesorCurrentHilillo[processorNumber]->setText(message);
}

void MainWindow::updateClockCycle()
{
	this->ui->clockCounterScreen->display(++ this->currentClockCycle);
}













