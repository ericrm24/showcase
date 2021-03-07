# Levenshtein Distance

Finds most similar files stored in directories using the Levenshtein Distance algorithm.

The files can be specified as command line arguments. If folders are provided then the command compares every file in them, even recursively if indicated by the user.

Applications of the algorithm include spell checking, string matching, RNA/DNA sequencing and information retrieval.


## User manual

Basic usage:

```
$ levdist DIRS|FILES
```

Options:

```
	--help		Prints help for the command
	--version	Prints information about the command
-q,	--quiet		Do not print elapsed time
-Q,	--silent	Do not generate output at all
-r,	--recursive	Analyze files in subdirectories
-u,			Enable Unicode support, ASCII is default
-w W			Use W workers (threads)
```

For example, to compare every file in the directory /lyrics, including subdirectories, with 3 threads but without printing the elapsed time:

```
$ levdist -rq -w 3 ~/Documents/lyrics
```

## Building

Requisites:

1. A POSIX Unix operating system
2. A GCC-compatible compiler

A Makefile is included, which allows to carry out operations such as:

```
make debug		Generates the executable distlev with debug options
make release	Compiles the executable without debug options and with optimizations
make clean		Clears created files. Used before switching between debug and release versions
```

To build, run the `make release` command in the source folder.

Installation:

```
$ make install
```

This allows to run the command as any other Unix command.

Uninstalling:

```
$ make uninstall
```

## Testing

To run automated tests:

1. Create a new folder in the 'test' directory.
2. Create a text file with the arguments the levdist command requires. Name it args.txt.
3. Create a text file with any input the command may need. Name it input.txt.
4. Create a text file with the expected output of the command. Name it output.txt
5. Create a text file with the expected error output. Name it error.txt.
6. Include in the folder the files or directories to be compared.
7. Run the 'make test' command with the Makefile.


## Author:

Eric Rios Morales (ericrios24@gmail.com).

This is part of a course's assignment.

## License:

This project is licensed under the terms of the [MIT license](https://opensource.org/licenses/MIT).
