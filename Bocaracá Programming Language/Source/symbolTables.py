from bccTypes import types

NAME_LEN = 30
NAME_SUFFIX_LEN = 3
TYPE_LEN = 15
TYPE_SUFFIX_LEN = 3
MODIFIED_LEN = 10
MODIFIED_SUFFIX_LEN = 3
LIST_LEN = 12
LIST_SUFFIX_LEN = 3
V_LINES = 5
ARGC_LEN = 10
ARGC_SUFFIX_LEN = 3
ARGV_LEN = 100
ARGV_SUFFIX_LEN = 3
LINE_LENGTH = NAME_LEN + NAME_SUFFIX_LEN + TYPE_LEN + TYPE_SUFFIX_LEN + MODIFIED_LEN + MODIFIED_SUFFIX_LEN + LIST_LEN + LIST_SUFFIX_LEN + V_LINES
FUNC_LINE_LENGTH = NAME_LEN + NAME_SUFFIX_LEN + TYPE_LEN + TYPE_SUFFIX_LEN + ARGC_LEN + ARGC_SUFFIX_LEN + ARGV_LEN + ARGV_SUFFIX_LEN + V_LINES

def printHLine(length):
    print('{:->{}}'.format('', length))

class SymbolTable:
    def __init__(self):
        self.table = {}

    def lookup(self, name):
        pass

    def insert(self, name, record):
        self.table[name] = record
    
    def show(self):
        printHLine(LINE_LENGTH)
        line = '|' + '{:>{}}'.format('Name', NAME_LEN) + '{:>{}}'.format('', NAME_SUFFIX_LEN) + '|' + '{:>{}}'.format('Type', TYPE_LEN) + '{:>{}}'.format('', TYPE_SUFFIX_LEN) + '|' + '{:>{}}'.format('Modified', MODIFIED_LEN) + '{:>{}}'.format('', MODIFIED_SUFFIX_LEN) + '|'  + '{:>{}}'.format('List Type', LIST_LEN) + '{:>{}}'.format('', LIST_SUFFIX_LEN) + '|'
        print(line, sep = '')
        printHLine(LINE_LENGTH)
        for key in self.table:
            varName = str(key)
            record = self.table[key]
            varType = list(types.keys())[list(types.values()).index(record[0])]
            modified = ('M' if record[1] else '-')
            list_type = '-'
            if record[2] is not None:
                if isinstance(record[2], int):
                    list_type = list(types.keys())[list(types.values()).index(record[2])]
                else:
                    list_type = list(types.keys())[list(types.values()).index(record[2][1])]
            line = '|' + '{:>{}}'.format(varName, NAME_LEN) + '{:>{}}'.format('', NAME_SUFFIX_LEN) + '|' + '{:>{}}'.format(varType, TYPE_LEN) + '{:>{}}'.format('', TYPE_SUFFIX_LEN) + '|' + '{:>{}}'.format(modified, MODIFIED_LEN) + '{:>{}}'.format('', MODIFIED_SUFFIX_LEN) + '|' + '{:>{}}'.format(list_type, LIST_LEN) + '{:>{}}'.format('', LIST_SUFFIX_LEN) + '|'
            print(line, sep = '')
            printHLine(LINE_LENGTH)
            

class VariableTable(SymbolTable):
    # name | type | modified | list_type | value | ncols name (type, modified, list_type, value, ncols)
    def __init__(self, id):
        super().__init__()
        self.id = id
        self.parent_tables = []

    def insert(self, name, varType, modified = False, list_type = None, value = None, cols = 0):
        success = False
        #if self.table.get(name, None) is None:
        super().insert(name, (varType, modified, list_type, value, cols))
        success = True
        return success

    def insertList(self, name, varType, listType, modified = False, value = None, cols = 0):
        success = False
        #if self.table.get(name, None) is None:
        super().insert(name, (varType, modified, listType, value, cols))
        success = True
        return success

class FunctionTable(SymbolTable):
    # name | returnType | argc | argv | functionBody | underAnalysis | calls
    # arg : (varType, varName)
    def __init__(self):
        super().__init__()
    
    def lookup(self, name):
        return self.table.get(name, None)
    
    def insert(self, name, returnType, argc, argv, functionBody=None, underAnalysis=False, calls=0):
        check = self.table.get(name, None)
        if check is None:
            record = (returnType, argc, argv, functionBody, underAnalysis, calls)
            super().insert(name, record)
        else:
            raise Exception('Function ' + name + ' already declared')

    def show(self):
        print('Function table:')
        
        printHLine(FUNC_LINE_LENGTH)
        line = '|' + '{:>{}}'.format('Name', NAME_LEN) + '{:>{}}'.format('', NAME_SUFFIX_LEN) + '|' + '{:>{}}'.format('Return Type', TYPE_LEN) + '{:>{}}'.format('', TYPE_SUFFIX_LEN) + '|' + '{:>{}}'.format('Arg. C.', ARGC_LEN) + '{:>{}}'.format('', ARGC_SUFFIX_LEN) + '|' + '{:>{}}'.format('Arg. V.', ARGV_LEN) + '{:>{}}'.format('', ARGV_SUFFIX_LEN) + '|'
        print(line, sep = '')
        printHLine(FUNC_LINE_LENGTH)

        for key in self.table:
            funcName = str(key)
            record = self.table[key]
            funcType = list(types.keys())[list(types.values()).index(record[0])]
            argc = str(record[1])
            argv = ['-']
            if record[2] is not None:
                argv = record[2]
            line = '|' + '{:>{}}'.format(funcName, NAME_LEN) + '{:>{}}'.format('', NAME_SUFFIX_LEN) + '|' + '{:>{}}'.format(funcType, TYPE_LEN) + '{:>{}}'.format('', TYPE_SUFFIX_LEN) + '|' + '{:>{}}'.format(argc, ARGC_LEN) + '{:>{}}'.format('', ARGC_SUFFIX_LEN) + '|' + '{:>{}}'.format(', '.join(argv), ARGV_LEN) + '{:>{}}'.format('', ARGV_SUFFIX_LEN) + '|'
            print(line, sep = '')
            printHLine(FUNC_LINE_LENGTH)
    
    def getFunctionType(self, name):
        return list(types.keys())[list(types.values()).index(self.table[name][0])]

    def getArguments(self, name):
        return self.table[name][2]

    def defineType(self, name, returnType):
        oldRecord = self.table[name]
        newRecord = (returnType, oldRecord[1], oldRecord[2], oldRecord[3], oldRecord[4], oldRecord[5])
        self.table[name] = newRecord
    
    def saveBody(self, name, functionBody):
        oldRecord = self.table[name]
        # Cambiar functionBody en la tabla
        newRecord = (oldRecord[0], oldRecord[1], oldRecord[2], functionBody, oldRecord[4], oldRecord[5])
        self.table[name] = newRecord
    
    def getFunctionBody(self, name):
        functionRecord = self.table[name]
        # Devolver functionBody
        return functionRecord[3]
    
    def setUnderAnalysis(self, name, underAnalysis):
        oldRecord = self.table[name]
        # Cambiar underAnalysis en la tabla
        newRecord = (oldRecord[0], oldRecord[1], oldRecord[2], oldRecord[3], underAnalysis, oldRecord[5])
        self.table[name] = newRecord
    
    def getUnderAnalysis(self, name):
        functionRecord = self.table[name]
        # Devolver underAnalysis
        return functionRecord[4]
    
    def addCall(self, name):
        oldRecord = self.table[name]
        # Incrementar calls
        calls = oldRecord[5] + 1
        # Cambiar calls en la tabla
        newRecord = (oldRecord[0], oldRecord[1], oldRecord[2], oldRecord[3], oldRecord[4], calls)
        self.table[name] = newRecord
    
    def getCalls(self, name):
        functionRecord = self.table[name]
        # Devolver calls
        return functionRecord[5]

    def reportErrors(self):
        warnings = []
        for key in self.table:
            funcName = str(key)
            timesCalled = self.getCalls(funcName)
            if timesCalled == 0:
                warnings.append('Warning: Detected that the function {} was never called in your code'.format(funcName))
        return warnings

class VariableTables():
    def __init__(self):
        self.tables = {}
        # Agregar tabla de variables globales
        self.tables[0] = VariableTable(0)
        # Definir su id como tabla actual
        self.current_id = 0
        # Agregar tabla de built-in
    
    # Delegar a su tabla actual
    def lookup(self, name):
        result = self.current_table().table.get(name, None)
        if result is None:
            for table_id in reversed(self.current_table().parent_tables):
                result = self.tables[table_id].table.get(name, None)
                if result is not None:
                    break
        return result

    def idLookup(self, id, name):
        result = self.tables[id].table.get(name, None)
        if result is None:
            for table_id in reversed(self.tables[id].parent_tables):
                result = self.tables[table_id].table.get(name, None)
                if result is not None:
                    break
        return result
    
    def idSetModified(self, id, name, modified):
        foundId = id
        result = self.tables[id].table.get(name, None)
        if result is None:
            for table_id in reversed(self.tables[id].parent_tables):
                result = self.tables[table_id].table.get(name, None)
                if result is not None:
                    foundId = table_id
                    break
        if result != None:
            record = (result[0], modified, result[2], value, result[4])
            self.tables[foundId].table[name] = record

        return result

    def idSetCols(self, id, name, cols):
        foundId = id
        result = self.tables[id].table.get(name, None)
        if result is None:
            for table_id in reversed(self.tables[id].parent_tables):
                result = self.tables[table_id].table.get(name, None)
                if result is not None:
                    foundId = table_id
                    break
        if result != None:
            record = (result[0], result[1], result[2], result[3], cols)
            self.tables[foundId].table[name] = record

        return result

    def idGetCols(self, id, name):
        foundId = id
        value = None
        result = self.tables[id].table.get(name, None)
        if result is None:
            for table_id in reversed(self.tables[id].parent_tables):
                result = self.tables[table_id].table.get(name, None)
                if result is not None:
                    foundId = table_id
                    break
        if result != None:
            value = result[4]
        return value

    def idSetType(self, id, name, varType):
        foundId = id
        result = self.tables[id].table.get(name, None)
        if result is None:
            for table_id in reversed(self.tables[id].parent_tables):
                result = self.tables[table_id].table.get(name, None)
                if result is not None:
                    foundId = table_id
                    break
        if result != None:
            record = (varType, result[1], result[2], result[3], result[4])
            self.tables[foundId].table[name] = record

        return result

    def idSetValue(self, id, name, value):
        foundId = id
        result = self.tables[id].table.get(name, None)
        if result is None:
            for table_id in reversed(self.tables[id].parent_tables):
                result = self.tables[table_id].table.get(name, None)
                if result is not None:
                    foundId = table_id
                    break
        if result != None:
            record = (result[0], result[1], result[2], value, result[4])
            self.tables[foundId].table[name] = record

    def idGetValue(self, id, name):
        foundId = id
        value = None
        result = self.tables[id].table.get(name, None)
        if result is None:
            for table_id in reversed(self.tables[id].parent_tables):
                result = self.tables[table_id].table.get(name, None)
                if result is not None:
                    foundId = table_id
                    break
        if result != None:
            value = result[3]
        return value

    def idGetNames(self, id):
        result = [key for key in self.tables[id].table.keys()]
        return result
    
    def idGetType(self, id, name):
        result = self.tables[id].table[name][0]
        return list(types.keys())[list(types.values()).index(result)]
    
    def isGlobal(self, name):
        # Buscar en la tabla de globales
        result = self.tables[0].table.get(name, None)
        return result is not None

    # Delegar a su tabla actual
    def insert(self, name, varType, modified = False):
        result = self.current_table().table.get(name, None)
        if result is None:
            self.current_table().insert(name, varType, modified, cols = 0)
        else:
            raise Exception("Variable " + name + " declared twice in the same scope")

    def insertList(self, name, varType, listType, modified = False):
        result = self.current_table().table.get(name, None)
        if result is None:
            self.current_table().insertList(name, varType, listType, modified, cols = 0)
        else:
            raise Exception("Variable " + name + " declared twice in the same scope")

    def current_table(self):
        return self.tables[self.current_id]
    
    def initializeScope(self, id):
        # Revisar que id no esté repetido
        
        # Obtener scopes actuales
        current_parent_tables = self.current_table().parent_tables.copy()
        parent = self.current_id

        # Agregar nueva tabla
        self.tables[id] = VariableTable(id)
        self.current_id = id

        # Agregar el anterior como padre
        self.current_table().parent_tables = current_parent_tables
        self.current_table().parent_tables.append(parent)

    def finalizeScope(self):
        # Obtener parent anterior (último del stack)
        self.current_id = self.current_table().parent_tables[-1]

    def show(self):
        print('Variable table:')
        for table in self.tables:
            printHLine(LINE_LENGTH)
            line = 'Table ID: ' + str(table) + "\tHas access to: " + str(self.tables[table].parent_tables)
            print('|', '{:<{}}'.format(line, LINE_LENGTH-5), '|', sep = '')
            self.tables[table].show()
            print('\n')

def testVariableTable():
    variables = VariableTables()
    variables.initializeScope(123)
    variables.insert("Other", 1)
    variables.initializeScope(122)
    variables.insert("Otherm", 3)
    variables.finalizeScope()
    variables.finalizeScope()
    variables.insert("Other", 0)
    variables.idSetModified(122, 'Other', True)
    variables.show()
    variables.idSetType(122, 'Other', 3)
    variables.show()

#testVariableTable()