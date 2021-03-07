TEMPLATE = app
QT -= core
CONFIG += console
CONFIG -= qt app_bundle
QMAKE_CXXFLAGS += -pthread
LIBS += -pthread

HEADERS += \
	arguments.h \
	concurrency.h \
	dir.h \
	levdist.h \
	queue.h \
        levenshtein.h \
        distance.h

SOURCES += \
	arguments.c \
	concurrency.c \
	dir.c \
	levdist.c \
	main.c \
	queue.c \
        levenshtein.c \
        distance.c