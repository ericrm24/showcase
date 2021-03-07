#include "Interfaz/mainwindow.h"
#include <QApplication>

int main(int argc, char *argv[])
{
    QApplication processorSimulator(argc, argv);
    MainWindow mainWindow;
    mainWindow.show();

    return processorSimulator.exec();
}
