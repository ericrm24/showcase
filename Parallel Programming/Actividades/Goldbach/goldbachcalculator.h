#ifndef GOLDBACHCALCULATOR_H
#define GOLDBACHCALCULATOR_H

#include <QAbstractListModel>
#include <QVector>

class GoldbachWorker;

class GoldbachCalculator : public QAbstractListModel
{
    Q_OBJECT
    Q_DISABLE_COPY(GoldbachCalculator)

  protected:
    int fetchedRowCount = 0;
    QVector<GoldbachWorker*> goldbachWorkers;
    QVector< QVector<QString> > results;
    QVector<int> progress;
    int lastProgress = 0;
    int workers = 0;
    QVector<long long> sumCount;
    QVector<double> time;

  public:
    explicit GoldbachCalculator(QObject *parent = nullptr);
    void calculate(long long number);
    void stop();
    QVector<QString> getAllSums() const;
    int getTotalOfSums() const;

  public: // Overriden methods from QAbastractListModel
    virtual int rowCount(const QModelIndex &parent = QModelIndex()) const override;
    virtual QVariant data(const QModelIndex &index, int role = Qt::DisplayRole) const override;

  protected:
    virtual bool canFetchMore(const QModelIndex &parent) const override;
    virtual void fetchMore(const QModelIndex &parent) override;

  signals:
    void calculationDone(long long sumCount);
    void progressChanged(int progress);

  protected slots:
    void workerDone(int workerNumber);
    void checkProgress();
};

#endif // GOLDBACHCALCULATOR_H
