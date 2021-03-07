#ifndef QBARRIER_H
#define QBARRIER_H


#include <QWaitCondition>
#include <QMutex>
#include <QObject>

#include <pthread.h>

class QBarrier:public QObject
{
    Q_OBJECT
private:
    /// Determines wether a 1s wait should be performed before letting the threads pass
    bool slowMode;
	/// Is in charge of thread synchronization
	pthread_barrier_t barrier;
	/// Counts the amount of threads currently waiting on the barrier.
	size_t waitingThreads;
	/// Used to protect the counter variable used for signaling the passing of a clock cycle
	QMutex counterMutex;



public:
    /**
     * @brief QBarrier Builds a barrier for thread synchronization.
	 * @param parent As the barrier is a QObject, it receives a parent for memory management.
	 * @parent slowMode Wether the barrier should operate on slow mode or not.
     */

	QBarrier( QObject* parent, bool slowMode);

    /**
     * @brief wait Waits on the barrier for the other processors to arrive. When all processors arrive, the barrier is lifted and processors continue.
     * @param finished Should be true if this is the last time this thread calls the barrier. False ow.
     */
	void wait();

signals:
	/**
	 * @brief clockCyclePassed Is emited when a full clock cycle has passed, for the interface to update its display.
	 */
    void clockCyclePassed();
};

#endif // QBARRIER_H
