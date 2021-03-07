
# ------------------------------------------------------------
 # Equipo: Elite Four
 # Integrantes: Ismael G., Diego N., Christian A., Eric R.
 #
 # Creates the intermidiate representation of the code
 # ------------------------------------------------------------
import symbolTables
import inspect
from bccTypes import types
from bccTypes import escaped
from llvmlite import ir
import llvmlite
import llvmlite.binding as llvm
from ctypes import CFUNCTYPE, c_int, c_float

# Para mostrar el nombre de la función (si se quiere seguir el recorrido)
# 	print(inspect.getframeinfo(inspect.currentframe()).function)

class IR:

    def __init__(self, variables, functions, modName = "sample"):
        self.i8= ir.IntType(8)                      #Char
        self.i64 = ir.IntType(64)                   #Int
        self.i1 = ir.IntType(1)                     #Bool
        self.f64 = ir.DoubleType()                  #Double
        self.void = ir.VoidType()
        self.voidptr = ir.IntType(8).as_pointer()   #Pointer
        self.builder = None
        self.variables = variables
        self.functions = functions
        self.llvm_functions = {}
        self.modName = modName
        self.module = ir.Module(name = modName)
        self.printf = ir.Function(self.module, ir.FunctionType(self.i64, [self.voidptr], var_arg=True), name="printf")
        self.scanf = ir.Function(self.module, ir.FunctionType(self.i64, [self.voidptr], var_arg=True), name="scanf")

        self.default_constants = {
            self.i8 : ir.Constant(self.i8, 0),
            self.i64 : ir.Constant(self.i64, 0),
            self.i1 : ir.Constant(self.i1, 0),
            self.f64 : ir.Constant(self.f64, 0.),
        }

    def startGeneration(self, dictionary):
        # Agregar funciones builtin
        self.builtin()
        if dictionary.get('funcdecls', None):
            self.handle_funcdecls(dictionary['funcdecls'])
        self.statement_main(dictionary)
        self.builder = ir.IRBuilder(self.first_block)
        self.builder.branch(self.main_block)
        
        with open(self.modName + '.irbcc', 'w') as f:
            f.write(str(self.module))
    
    def statement_main(self, dictionary):
        self.main_type = ir.FunctionType(self.i64, [])
        self.main = ir.Function(self.module, self.main_type, name='__main__')
        self.first_block = self.main.append_basic_block()
        self.main_block = self.main.append_basic_block()
        self.builder = ir.IRBuilder(self.main_block)
        self.statements(dictionary['statements'])
        ret = ir.Constant(self.i64, 0)
        self.builder.ret(ret)
    
    def _getTypeFromStr(self, stringName):
        typ = None
        if stringName == 'INT':
            typ = self.i64
        elif stringName == 'FLOAT':
            typ = self.f64
        elif stringName == 'BOOLEAN':
            typ = self.i1
        elif stringName == 'STRING':
            typ = self.voidptr
        elif stringName == 'LIST':
            typ = self.i1
        elif stringName == 'UNDEFINED':
            typ = self.i1
        elif stringName == 'VOID':
            typ = self.void
        return typ

    def handle_funcdecls(self, dictionary):
        name = dictionary['funcdecl']['variable']['name']
        funcId = dictionary['funcdecl']['id']
        if(self.functions.getCalls(name) > 0):
            funcTyp = self._getTypeFromStr(self.functions.getFunctionType(name))
            arguments = self.functions.getArguments(name)
            argTyps = []
            if arguments:
                for arg in arguments:
                    argTyps.append(self._getTypeFromStr(self.variables.idGetType(funcId, arg)))
            variables = self.functions.getArguments(name)
            varTyps = []
            if variables:
                for var in variables:
                    varTyps.append(self._getTypeFromStr(self.variables.idGetType(funcId, var)))
            fnty = ir.FunctionType(funcTyp, argTyps)
            func = ir.Function(self.module, fnty, name=name)
            self.llvm_functions[name] = func
            oldModule = self.module
            self.module = ir.Module(name=f'func_{name}')
            block = func.append_basic_block()
            self.builder = ir.IRBuilder(block)
            if variables:
                for idx, var in enumerate(variables):
                    addr = self.allocate(var, varTyps[idx])
                    self.builder.store(func.args[idx], addr)
                    self.variables.idSetValue(funcId, var, addr)
            self.statements(dictionary['funcdecl']['function_body']['statements'])
            retval = dictionary['funcdecl']['function_body']['retval']
            if retval:
                self.builder.ret(self.value_func(retval))
            else:
                self.builder.ret_void()
            self.module = oldModule
        if dictionary.get('funcdecls', None):
            self.handle_funcdecls(dictionary['funcdecls'])

    def block(self, dictionary):
        aux = None
        block = self.builder.append_basic_block()
        self.builder.position_at_end(block)
        aux = self.statements(dictionary['statements'])
        if not isinstance(aux, ir.Block):
            aux = None
        return block, aux

    def statements(self, dictionary):
        block = None
        if dictionary is not None:
            block = self.statement(dictionary['statement'])
            if dictionary.get("statements", False):
                block = self.statements(dictionary['statements'])
        if not isinstance(block, ir.Block):
            block = None
        return block

    def statement(self, dictionary):
        #print(dictionary)
        block = None
        block = getattr(self, dictionary["type"])(dictionary)
        if not isinstance(block, ir.Block):
            block = None
        return block

    def assignment_statement(self, dictionary):
        var_name = dictionary['variable']['name']
        value = self.value_func(dictionary['value'])
        var_type = self.get_var_type(value)
        addr = None

        if hasattr(value, 'type') and value.type == ir.ArrayType(ir.IntType(8), 255):
            var_type = self.voidptr
            string = ir.Constant(ir.ArrayType(ir.IntType(8), 255), [0] * 255)
            string_addr = self.builder.alloca(string.type)
            value = self.builder.store(value, string_addr)
            value = self.builder.bitcast(string_addr, self.voidptr)

        if type(value) == list:
            literal_value_array = self.get_literal_value_array(value)
            value = ir.Constant(var_type, literal_value_array)
            addr = self.variables.idGetValue(dictionary['id'], var_name)
        else:
            addr = self.variables.idGetValue(dictionary['id'], var_name)

        if "index" in dictionary:
            if "indey" in dictionary:
                #CASO DE UNA MATRIZ
                #indeyType = self.expression(dictionary['indey'])
                result = self.variables.idLookup(dictionary['id'], dictionary['variable']['name'])
                cols = ir.Constant(self.i64, result[4])
                index = self.expression(dictionary['index'])
                indey = self.expression(dictionary['indey'])
                zero = ir.Constant(self.i64, 0)
                realindex = self.builder.mul(cols, index)
                realindex = self.builder.add(realindex, indey)
                addr = self.builder.gep(ptr=addr, indices=[zero, realindex])
                addr = self.builder.bitcast(addr, var_type.as_pointer())
            else:
                #CASO DE UN ARREGLO
                offset = self.expression(dictionary['index'])
                zero = ir.Constant(self.i64, 0)
                addr = self.builder.gep(ptr=addr, indices=[zero, offset])
                addr = self.builder.bitcast(addr, value.type.as_pointer())
            # Es una variable normal
        self.builder.store(value, addr)
        return value

    def declaration(self, dictionary):
        var_name = dictionary['variable']['name']
        value = self.value_func(dictionary['value'])
        var_type = self.get_var_type(value)
        addr = None

        if hasattr(value, 'type') and value.type == ir.ArrayType(ir.IntType(8), 255):
            var_type = self.voidptr
            string = ir.Constant(ir.ArrayType(ir.IntType(8), 255), [0] * 255)
            string_addr = self.builder.alloca(string.type)
            value = self.builder.store(value, string_addr)
            value = self.builder.bitcast(string_addr, self.voidptr)

        # Si es global
        isGlobal = self.variables.isGlobal(var_name)
        if isGlobal and var_type != self.voidptr:
            addr = self.create_global(self.module, var_type, var_name, value)
        else:
            # Reservar memoria en stack
                addr = self.allocate(var_name, var_type)
        # Store
        if type(value) == list:
            if type(value[0]) == list:
                self.store_matrix(dictionary, var_name, value, addr, var_type)
            else:
                self.store_array(value, addr, var_type)
        else:
            addr = self.store_single_variable(dictionary, var_name, value, addr)
        self.variables.idSetValue(dictionary['id'], var_name, addr)
        return value

    def get_var_type(self, value):
        if type(value) == list:
            if type(value[0]) == list:
                return ir.ArrayType(value[0][0].type, len(value)*len(value[0]))
            else:
                return ir.ArrayType(value[0].type, len(value))
        else:
            return value.type

    def get_literal_value_array(self, values):
        literal_value_array = []
        for value in values:
            literal_value_array.append(value.constant)
        return literal_value_array

    def store_array(self, values, addr, var_type):
        # Store
        literal_value_array = self.get_literal_value_array(values)

        zero = ir.Constant(self.i64, 0)
        offset = 0

        for value in values:
            index = ir.Constant(self.i64, offset)
            aux = self.builder.gep(ptr=addr, indices=[zero, index])
            aux = self.builder.bitcast(aux, value.type.as_pointer())
            self.builder.store(value, aux)
            offset += 1

    def store_matrix(self, dictionary, var_name, values, addr, var_type):
        # Store
        literal_value_array = []
        for value in values:
            for element in value:
                literal_value_array.append(element.constant)

        size = []
        for row in range( len(values) ):
            size.append(len(values[row]))

        self.variables.idSetCols(dictionary['id'], var_name, size[0])

        for row in range( len(values) ):
            for col in range ( len(values[row]) ):
                zero = ir.Constant(self.i64, 0)
                index = ir.Constant(self.i64, size[0]*row+col)
                aux = self.builder.gep(ptr=addr, indices=[zero, index])
                aux = self.builder.bitcast(aux, values[row][col].type.as_pointer())
                self.builder.store(values[row][col], aux)

    def store_single_variable(self, dictionary, var_name, value, addr):
        # Store
        self.builder.store(value, addr)
        return addr

    def create_global(self, module, var_type, var_name, value):
        addr = ir.GlobalVariable(module, var_type, var_name)
        addr.initializer = self.default_constants.get(var_type) or self.manage_list_type(var_type)
        return addr

    def manage_list_type(self, var_type):
        return ir.Constant(ir.ArrayType(var_type.element, var_type.count), [0] * var_type.count)

    def allocate(self, name, varType):
        # Allocate
        with self.builder.goto_entry_block():
            alloca = self.builder.alloca(varType, size=None, name=name)
        return alloca

    def funccall_statement(self, dictionary):
        self.function_call(dictionary['funccall'])

    def function_call(self, dictionary):
        func = None
        if type(dictionary['functionname']) is dict:
            func = self.llvm_functions[dictionary['functionname']['name']]
        else:
            func = self.llvm_functions[dictionary['functionname']]
        # print(dictionary['arguments'])
        args = []
        if dictionary['arguments']['values']:
            for value in dictionary['arguments']['values']:
                args.append(self.value_func(value))
        return self.builder.call(func, args)

    def variable_func(self, dictionary):
        var_name = dictionary['name']
        addr = self.variables.idGetValue(dictionary['id'], var_name)
        load = None
        if self.is_string(addr):
            load = addr
        else:
            load = self.builder.load(addr)
        return load
    
    def boolean_func(self, dictionary):
        return ir.Constant(self.i1, 1 if dictionary['state'] == 'true' else 0)

    def int_func(self, dictionary):
        return ir.Constant(self.i64, dictionary['number'])

    def float_func(self, dictionary):
        return ir.Constant(self.f64, dictionary['number'])

    def string_func(self, dictionary):
        # Quitar comillas y manejar caracteres especiales como \n
        processed_string = self.process_string(dictionary['text'][1:-1])
        byte_string = bytearray(processed_string.encode(encoding='UTF-8'))
        
        string = ir.Constant(ir.ArrayType(self.i8, len(byte_string)), byte_string)
        str_addr = self.builder.alloca(string.type)
        self.builder.store(string, str_addr)
        # Convertir en puntero
        str_ptr = self.builder.bitcast(str_addr, self.voidptr)
        return str_ptr
    
    def string_append(self, lhs, rhs):
        print("String append")

    def expression(self, dictionary):
        expression = None
        if dictionary['type'] == 'termexpr':
            expression = self.term(dictionary['term'])
        elif dictionary['type'] == 'binopexpr':
            expression = self.binopexpr(dictionary)
        else:
            raise Exception('Error: Dictionary type is neither termexpr or binopexpr')
        return expression

    def binopexpr(self, dictionary):
        lhs = self.expression(dictionary['expression'])
        rhs = self.term(dictionary['term'])
        lhs, rhs = self.promote_numeric(lhs, rhs)
        expression = None
        op = dictionary['op']
        if op == '+' and ( self.is_string(lhs) or self.is_string(rhs) ):
            # String append
            # Si alguno es string
            expression = self.string_append(lhs, rhs)
        elif op == 'and':
            expression = self.builder.and_(lhs, rhs)
        elif op == 'or':
            expression = self.builder.or_(lhs, rhs)
        # Operaciones enteros
        elif lhs.type == self.i64:
            expression = self.binop_int_op(lhs, rhs, op)
        # Operaciones doubles
        elif lhs.type == self.f64:
            expression = self.binop_double_op(lhs, rhs, op)
        return expression
    
    def binop_int_op(self, lhs, rhs, op):
        expresion = None
        if op == '+':
            expression = self.builder.add(lhs, rhs)
        elif op == '-':
            expression = self.builder.sub(lhs, rhs)
        elif op == 'is':
            expression = self.builder.icmp_signed('==', lhs, rhs)
        elif op == 'isnot':
            expression = self.builder.icmp_signed('!=', lhs, rhs)
        else:
            expression = self.builder.icmp_signed(op, lhs, rhs)
        return expression
    
    def binop_double_op(self, lhs, rhs, op):
        expresion = None
        if op == '+':
            expression = self.builder.fadd(lhs, rhs)
        elif op == '-':
            expression = self.builder.fsub(lhs, rhs)
        elif op == 'is':
            expression = self.builder.fcmp_ordered('==', lhs, rhs)
        elif op == 'isnot':
            expression = self.builder.fcmp_ordered('!=', lhs, rhs)
        else:
            expression = self.builder.fcmp_ordered(op, lhs, rhs)
        return expression

    def primary(self, dictionary):
        #[int_func, float_func, variable_func, boolean_func, funccall, listsubs, expression]
        # Las funciones para el análisis semántico de funccall y listsubs no necesariamente dan excepción.
        result_type = -1
        if dictionary['type'] == 'literal':
            primary_dictionary = dictionary['value']
            if(primary_dictionary['type'] == "boolean"):
                return self.boolean_func(primary_dictionary)
            elif(primary_dictionary['type'] == "int"):
                return self.int_func(primary_dictionary)
            elif(primary_dictionary['type'] == "float"):
                return self.float_func(primary_dictionary)
            elif(primary_dictionary['type'] == 'funccall'):
                return self.function_call(primary_dictionary)
            elif(primary_dictionary['type'] == 'variable'):
                return self.variable_func(primary_dictionary)
            elif(primary_dictionary['type'] == 'string'):
                return self.string_func(primary_dictionary)
            elif(primary_dictionary['type'] == 'listsubs'):
                return self.listsubs_func(primary_dictionary)
            elif(primary_dictionary['type'] == 'matrixsubs'):
                return self.matrixsubs_func(primary_dictionary)
            else:
                raise Exception('wut') 
        else:
            return self.expression(dictionary['expression'])

    def term(self, dictionary):
        if dictionary['type'] == 'factorterm':
            return self.factor(dictionary['factor'])
        elif dictionary['type'] == 'binopterm':
            # times(*) divide(/) mod(%)
            # "type":"binopterm", "term":p[1], "op":p[2], "factor":p[3]

            term = self.term(dictionary['term'])
            op_type = dictionary['op']
            factor = self.factor(dictionary['factor'])
            term, factor = self.promote_numeric(term, factor)
            if term.type == self.i64:
                return self.term_int_op(term, factor, op_type)
            elif term.type == self.f64:
                return self.term_double_op(term, factor, op_type)
            else:
                raise Exception('invalid type found at operation')
        else:
            raise Exception('invalid operand found at factor')

    def term_int_op(self, term, factor, op_type):
        if op_type == '*':
            # asumiendo lo que dice que retorna {result, overflow bit}
            return self.builder.mul(term, factor)
        elif op_type == '/':
            return self.builder.sdiv(term, factor)
        elif op_type == '%':
            return self.builder.srem(term, factor)
        else:
            raise Exception('Invalid operation found at factor')        

    def term_double_op(self, term, factor, op_type):
        if op_type == '*':
            # asumiendo lo que dice que retorna {result, overflow bit}
            return self.builder.fmul(term, factor)
        elif op_type == '/':
            return self.builder.fdiv(term, factor)
        elif op_type == '%':
            return self.builder.frem(term, factor)
        else:
            raise Exception('Invalid operation found at factor')

    def promote_numeric(self, a, b):
        a_type = a.type
        b_type = b.type
        if( a_type == b_type or a_type == self.voidptr or b_type == self.voidptr):
            return a, b
        else:
            if a_type == self.i64:
                a = self.builder.sitofp(a, self.f64)
            elif b_type == self.i64:
                b = self.builder.sitofp(b, self.f64)
            else:
                raise Exception('Promote numeric esta raro: a={} b={}'.format(a.type,b.type))
        return a, b
    
    def factor(self, dictionary):
        factor = None
        if dictionary['type'] == 'primary':
            factor = self.primary(dictionary['value'])
        elif dictionary['type'] == 'unopfactor':
            factor = self.factor(dictionary['factor'])
            op = dictionary['op']
            # -
            if op == '-':
                factor = self.builder.neg(factor, name="neg")
            # Not
            elif op == 'not':
                factor = self.builder.not_(factor, name="not")
            # Si no es un + hay error
            elif op != '+':
                raise Exception('Invalid operation found at factor')
        else:
            raise Exception('Invalid type found at factor')
        return factor

    def cond(self, dictionary):
        return self.expression(dictionary['condition'])

#       if dictionary.get('else', False):
#            self.else_f(dictionary['else'])
#        elif dictionary.get('elseifs', False):
#            self.elseifs(dictionary['elseifs'])

    def statement_if(self, dictionary):
        #self.block(dictionary['ifblock'])
        # calculo de la expresion condicional
        cond = self.cond(dictionary['cond'])
        cond_block = self.builder.block

        if_block, aux = self.block(dictionary['ifblock'])

        oldBuilder = self.builder
        else_block = 0
        after_block = 0
        if dictionary.get('else', False):
            else_block, after_block = self.else_f(dictionary['else'])
        elif dictionary.get('elseifs', False):
            else_block, after_block = self.elseifs(dictionary['elseifs'])
        else:
            else_block = after_block = self.builder.append_basic_block()            
        self.builder = oldBuilder
        
        # Creacion del bloque del if y lo que sigue
        if not if_block.is_terminated:
            self.builder.position_at_end(if_block)

        if not self.builder.block.is_terminated:
            self.builder.branch(after_block)

        self.builder.position_at_end(cond_block)

        # Crea el branch
        self.builder.cbranch(cond, if_block, else_block)

        if aux != None:
            self.builder = ir.IRBuilder(aux)
            self.builder.branch(after_block)

        #Apunta el builder al bloque después del if
        self.builder = ir.IRBuilder(after_block)

        return after_block

    def else_f(self, dictionary):
        
        # Creacion del bloque del else y el de las instrucciones después de este
        else_block, aux = self.block(dictionary['elseblock'])
        after_block = self.builder.append_basic_block()
        self.builder.branch(after_block)

        if aux != None:
            self.builder = ir.IRBuilder(aux)
            self.builder.branch(after_block)

        #Apunta el builder al bloque después del while
        self.builder = ir.IRBuilder(after_block)
        return else_block, after_block

    def elseifs(self, dictionary):

        # calculo de la expresion condicional
        cond_block = self.builder.append_basic_block()  

        if_block, aux = self.block(dictionary['elseifblock'])

        oldBuilder = self.builder
        else_block = 0
        after_block = 0 
        if dictionary.get('else', False):
            else_block, after_block = self.else_f(dictionary['else'])
        elif dictionary.get('elseifs', False):
            else_block, after_block = self.elseifs(dictionary['elseifs'])
        else:
            else_block = after_block = self.builder.append_basic_block()            
        self.builder = oldBuilder
        
        # Creacion del bloque del if y lo que sigue

        if not if_block.is_terminated:
            self.builder.position_at_end(if_block)

        if not self.builder.block.is_terminated:
            self.builder.branch(after_block)

        # Crea el branch
#
        self.builder.position_at_end(cond_block)
        cond = self.cond(dictionary['cond'])
        self.builder.cbranch(cond, if_block, else_block)

        if aux != None:
            self.builder = ir.IRBuilder(aux)
            self.builder.branch(after_block)

        #Apunta el builder al bloque después del if
#        self.builder = ir.IRBuilder(after_block)

        return cond_block, after_block 

    def for_f(self, dictionary):
        block = self.builder.block

        var_name = dictionary['variable']['name']
        from_ = self.expression(dictionary['from'])
        to = self.expression(dictionary['to'])
        var_type = from_.type
        # Reservar memoria
        addr = self.allocate(var_name, var_type)
        # Guardar en tabla de símbolos
        self.variables.idSetValue(dictionary['id'], var_name, addr)
        # Store
        self.builder.store(from_, addr)

        # Bloque condicional
        cond_block = self.builder.append_basic_block()
        self.builder.position_at_end(cond_block)

        addr = self.variables.idGetValue(dictionary['id'], var_name)

        var_value = self.builder.load(addr)
        cond = self.builder.icmp_signed('<=', var_value, to)
        
        # Crea el bloque del for y el bloque con las instrucciones después del for
        for_block, aux = self.block(dictionary['forblock'])
        after_block = self.builder.append_basic_block()

        # Realiza el incremento de la variable contadora dentro del for y hace branch a la condición
        one = ir.Constant(var_type, 1)
        var_value = self.builder.load(addr)
        new_value = self.builder.add(var_value, one)
        self.builder.store(new_value, addr)
        self.builder.branch(cond_block)

        # Crea el branch
        oldBuilder = self.builder
        self.builder = ir.IRBuilder(cond_block)
        self.builder.cbranch(cond, for_block, after_block)
        self.builder = oldBuilder

        self.builder = ir.IRBuilder(block)
        self.builder.branch(cond_block)

        if aux != None:
            self.builder = ir.IRBuilder(aux)
            if not self.builder.block.is_terminated:
                self.builder.branch(after_block)

        #Apunta el builder al bloque después del for
        self.builder = ir.IRBuilder(after_block)

    def while_f(self, dictionary):
        block = self.builder.block
        cond = dictionary['cond']

        # Bloque condicional
        cond_block = self.builder.append_basic_block()
        self.builder.position_at_end(cond_block)
        cond = self.cond(dictionary['cond'])
        
        # Crea el bloque del while y el bloque con las instrucciones después del while
        while_block, aux = self.block(dictionary['whileblock'])
        after_block = self.builder.append_basic_block()

        # Hace branch a la condición
        self.builder.branch(cond_block)

        # Crea el branch
        oldBuilder = self.builder
        self.builder = ir.IRBuilder(cond_block)
        self.builder.cbranch(cond, while_block, after_block)
        self.builder = oldBuilder

        self.builder = ir.IRBuilder(block)
        self.builder.branch(cond_block)

        if aux != None:
            self.builder = ir.IRBuilder(aux)
            if not self.builder.block.is_terminated:
                self.builder.branch(after_block)

        #Apunta el builder al bloque después del while
        self.builder = ir.IRBuilder(after_block)

    def function_block(self, dictionary):
        pass

    def builtin(self):
        self.builtin_writeText()
        self.builtin_writeNumber()
        self.builtin_readText()
        self.builtin_readNumber()

    def builtin_writeText(self):
        name = 'writeText'
        # Write es tipo void y recibe un puntero
        fnty = ir.FunctionType(ir.VoidType(), [self.voidptr])
        func = ir.Function(self.module, fnty, name=name)

        self.llvm_functions[name] = func
        block = func.append_basic_block()
        self.builder = ir.IRBuilder(block)

        # Printf
        printf = self.printf

        # String para printf
        print_str = "%s\0"
        print_str_val = ir.Constant(ir.ArrayType(ir.IntType(8), len(print_str)), bytearray(print_str.encode("utf8")))
        print_str_addr = self.builder.alloca(print_str_val.type)
        self.builder.store(print_str_val, print_str_addr)
        print_arg = self.builder.bitcast(print_str_addr, self.voidptr)

        # Call printf
        self.builder.call(printf, [print_arg, func.args[0]])

        # Final
        self.builder.ret_void()

    def builtin_readText(self):
        name = 'readText'
        # readText es tipo STRING y no recibe parámetros
        fnty = ir.FunctionType(ir.ArrayType(ir.IntType(8), 255), [])
        func = ir.Function(self.module, fnty, name=name)

        self.llvm_functions[name] = func
        block = func.append_basic_block()
        self.builder = ir.IRBuilder(block)

        # Scanf
        scanf = self.scanf

        # String para scanf
        scan_str = "%254[^\n]"
        scan_str_val = ir.Constant(ir.ArrayType(ir.IntType(8), len(scan_str)), bytearray(scan_str.encode("utf8")))
        scan_str_addr = self.builder.alloca(scan_str_val.type)
        self.builder.store(scan_str_val, scan_str_addr)
        scan_arg = self.builder.bitcast(scan_str_addr, self.voidptr)

        # Variable donde se guarda lo leído
        scanned = ir.Constant(ir.ArrayType(ir.IntType(8), 255), [0] * 255)
        scanned_addr = self.builder.alloca(scanned.type)
        scanned_arg = self.builder.bitcast(scanned_addr, self.voidptr)

        # Call scanf
        read_text = self.builder.call(scanf, [scan_arg, scanned_arg])

        text = self.builder.load(scanned_addr)

        # Final
        self.builder.ret(text)

    def builtin_readNumber(self):
        name = 'readNumber'
        # readNumber es tipo FLOAT y no recibe parámetros
        fnty = ir.FunctionType(self.i64, [])
        func = ir.Function(self.module, fnty, name=name)

        self.llvm_functions[name] = func
        block = func.append_basic_block()
        self.builder = ir.IRBuilder(block)

        # Scanf
        scanf = self.scanf

        # String para scanf
        scan_str = "%ld"
        scan_str_val = ir.Constant(ir.ArrayType(ir.IntType(8), len(scan_str)), bytearray(scan_str.encode("utf8")))
        scan_str_addr = self.builder.alloca(scan_str_val.type)
        self.builder.store(scan_str_val, scan_str_addr)
        scan_arg = self.builder.bitcast(scan_str_addr, self.voidptr)

        # Variable donde se guarda lo leído
        scanned = ir.Constant(self.i64, 0)
        scanned_addr = self.builder.alloca(scanned.type)
        scanned_arg = self.builder.bitcast(scanned_addr, self.i64.as_pointer())

        # Call scanf
        read_num = self.builder.call(scanf, [scan_arg, scanned_arg])

        number = self.builder.load(scanned_addr)
        # Final
        self.builder.ret(number)

    def builtin_len(self, dictionary):
        pass

    def builtin_append(self, dictionary):
        pass

    def builtin_remove(self, dictionary):
        pass
    
    def builtin_writeNumber(self):
        name = 'writeNumber'
        # WriteNumber es tipo void y recibe un entero
        fnty = ir.FunctionType(ir.VoidType(), [self.i64])
        func = ir.Function(self.module, fnty, name=name)

        self.llvm_functions[name] = func
        block = func.append_basic_block()
        self.builder = ir.IRBuilder(block)

        # Printf
        printf = self.printf

        # String para printf
        print_str = "%i\0"
        print_str_val = ir.Constant(ir.ArrayType(ir.IntType(8), len(print_str)), bytearray(print_str.encode("utf8")))
        print_str_addr = self.builder.alloca(print_str_val.type)
        self.builder.store(print_str_val, print_str_addr)
        print_arg = self.builder.bitcast(print_str_addr, self.voidptr)

        # Call printf
        self.builder.call(printf, [print_arg, func.args[0]])

        # Final
        self.builder.ret_void()

    def value_func(self, dictionary):
        if dictionary['type'] == 'expression_value':
            return self.expression(dictionary['content'])
        elif dictionary['type'] == 'value_list':
            value_list = []
            for value in dictionary['content']:
                list_element = self.value_func(value)
                value_list.append(list_element)
            return value_list

    def list_subs(self, dictionary):
        pass

    def value_expr(self, dictionary):
        return self.expression(dictionary['content'])
    
    def is_string(self, value):
        value_len = 0
        if hasattr(value, 'constant') and hasattr(value.constant, '__len__'):
            value_len = len(value.constant)
        return (type(value) != list) and (value.type == self.voidptr or value.type == ir.ArrayType(self.i8, value_len))
    
    def process_string(self, text):
        text += '\0'
        for escape in escaped:
            if escape in text:
                text = text.replace(escape, escaped[escape])
        return text

    def listsubs_func(self, dictionary):
        var_name = dictionary['variable']['name']
        var_type = self.variables.idLookup(dictionary['id'], dictionary['variable']['name'])[2]
        var_type = self._getTypeFromStr(list(types.keys())[list(types.values()).index(var_type)])
        addr = self.variables.idGetValue(dictionary['id'], var_name)
        offset = self.expression(dictionary['index'])
        zero = ir.Constant(self.i64, 0)
        addr = self.builder.gep(ptr=addr, indices=[zero, offset])
        addr = self.builder.bitcast(addr, var_type.as_pointer())
        value = self.builder.load(addr)
        return value

    def matrixsubs_func(self, dictionary):
        var_name = dictionary['variable']['name']
        result = self.variables.idLookup(dictionary['id'], dictionary['variable']['name'])
        var_type = result[2]
        cols = ir.Constant(self.i64, result[4])
        var_type = self._getTypeFromStr(list(types.keys())[list(types.values()).index(var_type)])
        addr = self.variables.idGetValue(dictionary['id'], var_name)
        
        #
        index = self.expression(dictionary['index'])
        indey = self.expression(dictionary['indey'])
        zero = ir.Constant(self.i64, 0)
        realindex = self.builder.mul(cols, index)
        realindex = self.builder.add(realindex, indey)
        addr = self.builder.gep(ptr=addr, indices=[zero, realindex])
        addr = self.builder.bitcast(addr, var_type.as_pointer())
        value = self.builder.load(addr)
        return value



