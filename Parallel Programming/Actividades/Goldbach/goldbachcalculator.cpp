#include "goldbachcalculator.h"
#include "goldbachworker.h"

#include <iostream>

GoldbachCalculator::GoldbachCalculator(QObject *parent)
    : QAbstractListModel(parent)
{
}

void GoldbachCalculator::calculate(long long number)
{
    this->beginResetModel();

    workers = QThread::idealThreadCount() - 1;
    this->results.resize(workers);
    this->progress.resize(workers);
    this->sumCount.resize(workers);
    this->time.resize(workers);
    for ( int current = 0; current < workers; ++current  )
    {
        this->goldbachWorkers.append( new GoldbachWorker(number, current, workers, this->results[current], this->progress[current], this->sumCount[current], this->time[current], this) );
//        this->connect( this->goldbachWorker, &GoldbachWorker::sumFound, this, &MainWindow::appendResult );
        this->connect( this->goldbachWorkers[current], &GoldbachWorker::calculationDone, this, &GoldbachCalculator::workerDone );
        this->connect(this->goldbachWorkers[current], &GoldbachWorker::progressChanged, this, &GoldbachCalculator::checkProgress);
        this->goldbachWorkers[current]->start();
    }

    this->fetchedRowCount = 0;
    this->endResetModel();
}

void GoldbachCalculator::stop()
{
   for (int i = 0; i < workers; ++i) {
       Q_ASSERT( this->goldbachWorkers[i] );
       this->goldbachWorkers[i]->requestInterruption();
   }

}

QVector<QString> GoldbachCalculator::getAllSums() const
{
    QVector<QString> allSums;
    int count = 1;

    for (int i = 0; i < this->results.size(); ++i){
        for ( int j = 0; j < this->results[i].size(); ++j ) {
            allSums.append( tr("%1: %2" ).arg(count++).arg( this->results[i][j] ));
        }
    }

    return allSums;
}

int GoldbachCalculator::getTotalOfSums() const
{
    int total = 0;
    for ( int i = 0; i < this->results.size(); ++i){
        total += this->results[i].size();
    }
    return total;
}

int GoldbachCalculator::rowCount(const QModelIndex &parent) const
{
    Q_UNUSED(parent);
    return this->fetchedRowCount;
}

QVariant GoldbachCalculator::data(const QModelIndex &index, int role) const
{
    if ( ! index.isValid() )
        return QVariant();

    if ( index.row() < 0 || index.row() >= this->getTotalOfSums() )
        return QVariant();

    if (role == Qt::DisplayRole)
    {
        int count = index.row();
        for (int i = 0; i < this->results.size(); ++i) {
            if (this->results[i].size() <= count)
                count -= this->results[i].size();

            else
                return tr("%1: %2").arg(index.row() + 1).arg(this->results[ i ] [ count ]);

        }
    }

    return QVariant();
}

bool GoldbachCalculator::canFetchMore(const QModelIndex &parent) const
{
    Q_UNUSED(parent);
    return this->fetchedRowCount < this->getTotalOfSums();
}

void GoldbachCalculator::fetchMore(const QModelIndex &parent)
{
    Q_UNUSED(parent);
    int remainder = this->getTotalOfSums() - this->fetchedRowCount;
    int itemsToFetch = qMin(100, remainder);

    if (itemsToFetch <= 0)
        return;

    int firstRow = this->fetchedRowCount;
    int lastRow = this->fetchedRowCount + itemsToFetch - 1;

    beginInsertRows(QModelIndex(), firstRow, lastRow);
    this->fetchedRowCount += itemsToFetch;
    endInsertRows();
}

void GoldbachCalculator::workerDone(int workerNumber)
{
    this->goldbachWorkers[workerNumber]->deleteLater();
    this->goldbachWorkers[workerNumber] = nullptr;

    bool allDone = true;
    for(int i = 0; i < workers && allDone; ++i)
        allDone = this->goldbachWorkers[i] == nullptr;
    if (allDone){
        int totalSums = 0;
        for(int i = 0; i < this->sumCount.size(); ++i){
            totalSums += sumCount[i];
        }

        /* Do not execute 'for' when running Tester */
        #ifndef TESTING
            for ( int i = 0; i < this->workers; ++i ) {
                std::cout << "Worker " << i + 1 << " took " << this->time[i] << " seconds and found " << this->sumCount[i] << " sums." << std::endl << std::endl;
            }
            std::cout << std::endl;
        #endif
        emit calculationDone(totalSums);
    }

}

void GoldbachCalculator::checkProgress()
{
    int totalProgress = 0;
    for ( int i = 0; i < this->progress.size(); ++i)
        totalProgress += this->progress[i];

    totalProgress /= this->workers;

    if (totalProgress != this->lastProgress){
        this->lastProgress = totalProgress;
        emit progressChanged(totalProgress);
    }
}
