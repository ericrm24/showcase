#include "goldbachtester.h"

#include <iostream>

int main( int argc, char** argv)
{
    #ifndef RELEASE
       std::cerr << "This is the Tester" << std::endl;
    #endif

    if ( argc > 1) {
        GoldbachTester tester(argc, argv);
        return tester.run();
    }
    else {
        std::cerr << "Usage: GoldbachTester test_folder1 test_folder2 ... test_folderN" << std::endl;
        return 0;
    }
}
