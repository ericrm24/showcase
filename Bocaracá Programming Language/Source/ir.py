from llvmlite import ir
import llvmlite
import llvmlite.binding as llvm
from ctypes import CFUNCTYPE, c_int, c_float

llvm.initialize()
llvm.initialize_native_target()
llvm.initialize_native_asmprinter()

i32 = ir.IntType(32)
i64 = ir.IntType(64)
i1 = ir.IntType(1)
f32 = ir.FloatType()
f64 = ir.DoubleType()

symbol_table = {}

#make a module
module = ir.Module(name = "example")

# define function parameters for function "main"
return_type = i32 #return int
argument_types = list() #can add ir.IntType(#), ir.FloatType() for arguments
func_name = "main"

#make a function
fnty = ir.FunctionType(return_type, argument_types)
main_func = ir.Function(module, fnty, name=func_name)

# append basic block named 'entry', and make builder
# blocks generally have 1 entry and exit point, with no branches within the block
block = main_func.append_basic_block('entry')
builder = ir.IRBuilder(block)

def ir_type(string):
    if "int" in string:
        return i32
    if "float" in string:
        return f32
    if "boolean" in string:
        return i1
    return ir.VoidType()


def generate(ast):
    retVar = -1
    if(ast["type"] == "statements"):
        generate(ast["statement"])
        generate(ast["statements"])
    elif(ast["type"] == "statement"):
        generate(ast["statement"])
    elif(ast["type"] == "assignment"):
        retVar = generate(ast["value"])
        symbol_table[ast["varName"]] = retVar
    elif(ast["type"] == "expression"):
        retVar = generate(ast["content"])
    elif(ast["type"] == "binop"):
        #Cuando se introduzcan operadores lógicos hay que cambiar esto para que haga un chequeo.
        lhs = generate(ast["lhs"])
        lhs = ir.Constant(lhs["type"], lhs["value"])
        rhs = generate(ast["rhs"])
        rhs = ir.Constant(rhs["type"], rhs["value"])
        op = ast["op"]["op"]
        if (op == "+"):
            retVar = {"type":i32, "value":builder.add(lhs, rhs, name="add")}
    elif(ast["type"] == "boolean"):
        retVar = {"type":ir_type("boolean"), "value":(1 if ast["value"] == "true" else 0)}
    elif(ast["type"] == "int"):
        retVar = {"type":ir_type("int"), "value":ast["value"]}
    return retVar

def mainFunc(ast):
    generate(ast)
    print("\n")
    print("IR")
    print("Symbol table = ", symbol_table)
    print("\n")
    print(module)
    return module


if __name__ == '__main__':
    cualquiera = ir.Constant(i32, 10)
    cualquiera2 = ir.Constant(i32, 100)
    resultado = builder.add(cualquiera, cualquiera2, name="add")
    resultado2 = builder.add(resultado, cualquiera2, name="add2")
    print(module)