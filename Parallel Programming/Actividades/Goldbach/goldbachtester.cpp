#include "goldbachtester.h"
#include "goldbachcalculator.h"

#include <iostream>
#include <QDir>
#include <QFile>
#include <QTextStream>

GoldbachTester::GoldbachTester(int &argc, char **argv)
  : QCoreApplication(argc, argv)
{

}

int GoldbachTester::run()
{
    for ( int index = 1; index < this->arguments().count(); ++index )
        this->testDirectory(this->arguments()[index]);
        //std::cout << index << ": " << qPrintable(this->arguments()[index]) << std::endl;

    return this->exec();
}

bool GoldbachTester::testDirectory(const QString &path)
{
    QDir dir (path);
    if ( ! dir.exists())
        return std::cerr << "error: directory not found: " <<qPrintable(path) << std::endl, false;

    //std::cout << "Directory: " << qPrintable(path) << std::endl;

    dir.setFilter(QDir::Files);

    QFileInfoList fileList = dir.entryInfoList();
    for (int index = 0; index < fileList.count(); ++index){
        ++this->testsCount;
        this->testFile( fileList[index] );
        //std::cout << "Proccessing file " << qPrintable( fileList[index].absoluteFilePath() ) << std::endl;
    }

    return true;
}

bool GoldbachTester::testFile(const QFileInfo &fileInfo)
{
    bool ok = true;
    long long number = fileInfo.baseName().toLong(&ok);
    if ( ok == false )
        return std::cerr << "error: invalid number: " << qPrintable(fileInfo.baseName()) << std:: endl, false;

    GoldbachCalculator* goldbachCalculator = new GoldbachCalculator(this);
    this->calculators.insert( goldbachCalculator, fileInfo );
    this->connect (goldbachCalculator, &GoldbachCalculator::calculationDone, this, &GoldbachTester::calculationDone);
    goldbachCalculator->calculate(number);

    return true;
}

QVector<QString> GoldbachTester::loadLines(const QFileInfo &fileInfo)
{
    QVector<QString> result;
    QFile file( fileInfo.absoluteFilePath() );
    if ( ! file.open(QFile::ReadOnly) )
        return std::cerr << "error: could not open: " << qPrintable(fileInfo.fileName()), result;

    QTextStream textStream(&file);
    QString line;
    while ( ! textStream.atEnd() ) {
        line = textStream.readLine();
        if ( ! line.isEmpty() )
            result.append( line.trimmed() );
    }

    return result;
}

void GoldbachTester::calculationDone(long long sumCount)
{
    Q_UNUSED(sumCount);

    GoldbachCalculator* goldbachCalculator = dynamic_cast<GoldbachCalculator*> ( sender() );
    Q_ASSERT(goldbachCalculator);

    QFileInfo fileInfo = this->calculators.value(goldbachCalculator);

    const QVector<QString>& calculatorSums = goldbachCalculator->getAllSums();
    const QVector<QString>& expectedSums = loadLines( fileInfo );
    this->compareResults(expectedSums, calculatorSums, fileInfo.baseName());
    /* if ( calculatorSums != expectedSums )
        std:: cerr << "Test case failed: " << qPrintable(fileInfo.fileName()) << std::endl;
    else
         ++this->testPassed;*/

    goldbachCalculator->deleteLater();
    this->calculators.remove(goldbachCalculator);
    if (this->calculators.count() <= 0){
        int successPercentage = (this->testPassed * 100) / this->testsCount;
        std::cout << this->testsCount << " tests cases: " << this->testPassed << " passed (" << successPercentage << "%), " << this->testsCount - this->testPassed << " failed (" << 100 - successPercentage << "%)." << std::endl;
        this->quit();
    }

    if ( this->calculators.count() <= 0 )
        this->quit();

}

void GoldbachTester::compareResults(const QVector<QString> &file, const QVector<QString> &results, const QString fileName)
{
    int index = 0;
    bool correct = true;

    while ( index < file.size() && index < results.size() && correct ) {
        correct = file[index] == results[index];
        ++index;
    }
    --index;


    if ( ! correct ) {
        std::cout << "test case "  << qPrintable(fileName) << " failed:" << std::endl;
        QString expected, produced;

        if ( ! file[index].isNull() )
            expected = file [index];

        if ( ! results[index].isNull() )
            produced = results[index];

        std::cout << "\texpected: " << qPrintable(expected) << std::endl;
        std::cout << "\tproduced: " << qPrintable(produced) << std::endl;
    }
    else {
        if ( file.size() < results.size() ) {
            std::cout << "test case "  << qPrintable(fileName) << " failed:" << std::endl;
            std::cout << "\textra line(s) in results" << std::endl;
        }
        else {
            if ( results.size() < file.size() ) {
                std::cout << "test case "  << qPrintable(fileName) << " failed:" << std::endl;
                std::cout << "\tmissing line(s) in results" << std::endl;
            }
            else
                ++this->testPassed;
        }
    }
}
