# ------------------------------------------------------------
 # Equipo: Elite Four
 # Integrantes: Ismael G., Diego N., Christian A., Eric R.
 #
 # Compiler
 #
 # Instrucciones de uso:
 #
 # 1. Correr con el comando
 #
 #          python3 compilerBoc.py file
 #
 # en donde "python3" es el intérprete de Python 3, "compiler.py" la implementación del 
 # compilador, y "file" el archivo que contiene el código fuente de entrada que se quiere 
 # analizar y compilar.
 # ------------------------------------------------------------

import sys
import argparse
import parserBcc as bocaracaParser
import symbolTables
from first_pass import FirstWalker
from second_pass import SecondWalker
from semAnalyzerBcc import SemAnalyzer
from irBcc import IR

def CreateArgP():
    argP = argparse.ArgumentParser()

    argP.add_argument('file', help='File to be compiled')
    argP.add_argument('-r', '--run', action='store_true', help='runs code on bccEngine')
    argP.add_argument('-v', '--verbosity', type=int, choices=[0,1,2,3,4], help='set output verbosity')

    return argP

# Verbosity:
#   0 -> nada
#   1 -> warnings
#   2 -> 1 + tablas de simbolos
#   3 -> 2 + árbol sintáctico
#   4 -> 3 + tokens

# Get the arguments
filename = None
run = False
verbosity = 0
argP = CreateArgP()
arguments = argP.parse_args(sys.argv[1:])
run = arguments.run
if arguments.verbosity:
    verbosity = arguments.verbosity
filename = arguments.file

 # Build the parser
parser = bocaracaParser.BocaracaParser(show_tokens = verbosity >= 4, show_ast = verbosity >= 3)
variables = symbolTables.VariableTables()
functions = symbolTables.FunctionTable()

outputFilename = filename.split('.')[0]
# Open file and read lines
fileRead = open(filename, 'r') 
lines = fileRead.readlines() 
lines = "".join(line for line in lines)

ast = parser.parse(lines)

if not parser.error:

        #print('First pass')
    try:

        firstWalker = FirstWalker(ast)

        firstWalker.walk()

    except Exception as e:
        print("First walker error")
        print(e)
        sys.exit()

    ast = firstWalker.dictionary

    #print('Second pass')

    secondWalker = SecondWalker(ast, variables, functions)

    try:
        secondWalker.walk()
    except Exception as e:
        print("Second walker error")
        print(e)
        sys.exit()

    variables = secondWalker.variables
    functions = secondWalker.functions

    try:
        #print('Last pass')

        semAnalyzer = SemAnalyzer(variables, functions)

        semAnalyzer.startAnalysis(ast)

    except Exception as e:
        print("Semantic analyzer error")
        print(e)
        sys.exit()

    # Mostrar tablas finales
    if verbosity >= 2:
        variables.show()
        functions.show()
        print()

    if verbosity >= 1:
        warnings = secondWalker.semAnalyzer.warnings + semAnalyzer.warnings + functions.reportErrors()
        for warning in warnings:
            print(warning)

    irGen = IR(variables, functions, outputFilename)

    #try:
    irGen.startGeneration(ast)
    #except Exception as e:
        #print("IR generator error")
        #print(e)
        #sys.exit()
    if(run):
        from bccEngine import BccEngine
        module = None
        with open(outputFilename+'.irbcc', 'r') as f:
            module = f.read()
        bccEngine = BccEngine()
        bccEngine.execute(module)