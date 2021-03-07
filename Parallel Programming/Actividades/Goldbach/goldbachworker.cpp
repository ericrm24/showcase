#include <QtMath>

#include "goldbachworker.h"

GoldbachWorker::GoldbachWorker(long long number, int workerNumber, int workerCount, QVector<QString>& results, int& progress, long long &sumCount, double &time, QObject *parent)
    : QThread(parent)
    , number{number}
    , results{results}
    , progress{progress}
    , workerNumber{workerNumber}
    , workerCount {workerCount}
    , sumCount {sumCount}
    , time {time}
{
}

void GoldbachWorker::run()
{
    timer.start();
    this->sumCount = this->calculate(number);
    time = timer.elapsed() / 1000.0;
    this->progress = 100;
    emit this->calculationDone( this->workerNumber );
}

long long GoldbachWorker::calculate(long long number)
{
    if ( number < 4 || number == 5 ) return 0;
    return number % 2 == 0 ? calculateEvenGoldbach(number) : calculateOddGoldbach(number);
}

long long GoldbachWorker::calculateEvenGoldbach(long long number)
{
    long long results = 0;
    long long lastToCalculate = ( number / 2 );
    int jobForEachThread = qCeil( lastToCalculate / this->workerCount);
    int first = jobForEachThread * this->workerNumber;
    int last = jobForEachThread * (this->workerNumber + 1);
    if ( ( this->workerCount - this->workerNumber ) == 1 )
        last = lastToCalculate + 1;

    for ( long long a = first; a < last; ++a )
    {
        if ( ! isPrime(a) ) continue;
        long long b = number - a;
        if ( b >= a && isPrime(b) ) {
            this->results.append( tr("%1 + %2").arg(a).arg(b) );
            ++results;
        }

        // Update the progress bar
        this->progress = ((a + 1) * 100 / last);
        emit progressChanged();
        // If user cancelled, stop calculations
        if ( this->isInterruptionRequested() )
            return results;
    }
    return results;
}

long long GoldbachWorker::calculateOddGoldbach(long long number)
{
    long long results = 0;
    long long lastToCalculate = (number / 3);
    int jobForEachThread = qCeil(lastToCalculate / this->workerCount);
    int first = jobForEachThread * this->workerNumber;
    int last = jobForEachThread * (this->workerNumber + 1);
    if ( (this->workerCount - this->workerNumber) == 1)
        last = lastToCalculate + 1;

    for ( long long a = first; a < last; ++a )
    {
        if ( ! isPrime(a) ) continue;
        for ( long long b = a; b < number; ++b )
        {
            if ( ! isPrime(b) ) continue;
            long long c = number - a - b;
            if ( c >= b && isPrime(c) ) {
                this->results.append( tr("%1 + %2 + %3").arg(a).arg(b).arg(c) );
                ++results;
            }

            // Update the progress bar
           this->progress = ((a + 1) * 100 / last);
            emit progressChanged();
            // If user cancelled, stop calculations
            if ( this->isInterruptionRequested() )
                return results;
        }
    }
    return results;
}

bool GoldbachWorker::isPrime(long long number)
{
    if ( number < 2 ) return false;
    if ( number == 2) return true;
    if ( !(number & 1) ) return false;

    for ( long long i = 2, last = qSqrt(number); i <= last; ++i )
        if ( number % i == 0 )
            return false;

    return true;
}
