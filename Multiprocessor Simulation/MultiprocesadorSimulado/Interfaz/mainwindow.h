#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>
#include <QFileInfoList>
#include <QVector>
#include <QThread>
#include <QPlainTextEdit>
#include <QLineEdit>

class Processor;
class ClockManager;
class QBarrier;


namespace Ui
{
	class MainWindow;
}

class MainWindow : public QMainWindow
{
    Q_OBJECT
private:
	/// Class that stores user interface components.
	Ui::MainWindow *ui;

private:
	/// List of files selected for execution.
	QFileInfoList selectedFiles;
	/// Container for the processors created.
    QVector<Processor*> processors;
	/// Container for ease of use on processor logs
	QVector<QPlainTextEdit*> processorLogs;
	/// Container for ease of use on current activity displays
	QVector<QLineEdit*> procesorCurrentHilillo;
	/// Counts how many processors are fully finished. Used for final statistics display
    int finishedProcessors;
	/// Used for synchronization of clock cycles
    QBarrier* barrier;
	/// Stores the current clock cycle for it to be displayed on the interface.
	int currentClockCycle;

public:
	/**
	 * @brief MainWindow Default constructor for Main Window class
	 * @param parent For QObject initialization
	 */
    explicit MainWindow(QWidget *parent = nullptr);
	/**
	 * Destructor for main window.
	 */
    ~MainWindow();

	/**
	 * @brief openNewFolder Opens a dialog for the user to choose a folder, and processes its contents.
	 * @details The user may select a folder from the filesystem using a dialog. Once the folder is selected, its contents are analyzed looking for .txt files. The files are collected on a QFileInfoList.
	 * @return A QFileInfoList containing all of the .txt files on the selected folder.
	 */
	QFileInfoList openNewFolder();
	/**
	 * @brief getFinishedProcessors Getter function for finished processors atribute.
	 * @return The value of the finished processors atribute.
	 */
	int getFinishedProcessors() const;
	/**
	 * @brief getCurrentClockCycle Getter function for the currentClockCycle attribute.
	 * @return The value of the current clock cycle.
	 */
	size_t getCurrentClockCycle() const;

private slots:
	/**
	 * @brief on_startSimulationButton_clicked Called when the simulation should be started. Clears all old display information and creates the worker threads.
	 */
	void on_startSimulationButton_clicked();

	/**
	 * @brief on_openFolderButton_clicked Called when the user requests to open a folder. Finds any .txt file and adds it to a queue.
	 */
	void on_openFolderButton_clicked();
	/**
	 * @brief processorExecutionFinished Called when a processor is finished. Used to display final information and reset variables.
	 */
	void processorExecutionFinished();
	/**
	 * @brief processorFinishedProcessing Called when a processor is running 'hilillos'. Used for final statistics generation.
	 */
	void processorFinishedProcessing();
	/**
	 * @brief updateClockCycle Called when a clock cycle is finished for the interface to update the information on screen.
	 */
	void updateClockCycle();
	/**
	 * @brief addMessage Appends the provided string to the corresponding processor's log on screen.
	 * @param message The information desired to be added.
	 * @param processorNumber The processor to whose log needs to be updated.
	 */
	void addMessage(QString message, int processorNumber);
	/**
	 * @brief changeFile Changes the display on which file is being executed on a processor
	 * @param message The file name to be displayed
	 * @param processorNumber The processor whose current file has changed.
	 */
	void changeFile(QString message, int processorNumber);


};

#endif // MAINWINDOW_H
