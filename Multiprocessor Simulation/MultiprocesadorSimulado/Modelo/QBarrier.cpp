#include "QBarrier.h"

#include <QThread>

#include <iostream>

QBarrier::QBarrier(QObject* parent, bool slowMode)
    : QObject (parent)
	, slowMode(slowMode)
	, waitingThreads(0)
{
	pthread_barrier_init(&this->barrier, nullptr, 3);
}

void QBarrier::wait()
{
	if(slowMode)
		QThread::msleep(500);
	else
        QThread::usleep(1);
	this->counterMutex.lock();
		if((++this->waitingThreads) == 3)
		{
			emit this->clockCyclePassed();
			this->waitingThreads = 0;
		}
	this->counterMutex.unlock();
	pthread_barrier_wait(&this->barrier);
}
