
import sys
from llvmlite import ir
import llvmlite
import llvmlite.binding as llvm
from ctypes import CFUNCTYPE, c_int, c_float

class BccEngine:

    def create_execution_engine(self):
        """
        Create an ExecutionEngine suitable for JIT code generation on
        the host CPU.  The engine is reusable for an arbitrary number of
        modules.
        """
        # This is required
        llvm.initialize()
        llvm.initialize_native_target()
        llvm.initialize_native_asmprinter()

        # Create a target machine representing the host
        target = llvm.Target.from_default_triple()
        target_machine = target.create_target_machine()
        # And an execution engine with an empty backing module
        backing_mod = llvm.parse_assembly("")
        engine = llvm.create_mcjit_compiler(backing_mod, target_machine)
        return engine

    def compile_ir(self, engine, llvm_ir):
        """
        Compile the LLVM IR string with the given engine.
        The compiled module object is returned.
        """
        # Create a LLVM module object from the IR
        mod = llvm.parse_assembly(llvm_ir)
        mod.verify()
        # Now add the module and make sure it is ready for execution
        engine.add_module(mod)
        engine.finalize_object()
        engine.run_static_constructors()

    def execute(self, module):

        engine = self.create_execution_engine()
        self.compile_ir(engine, str(module))

        # Look up the function pointer (a Python int)
        func_ptr = engine.get_function_address("__main__")

        cfunc = CFUNCTYPE(c_int)(func_ptr)
        res = cfunc()

# Get the filename
if __name__ == '__main__':
    filename = None
    if len(sys.argv) < 2:
        filename = input("Enter the name of the file to be executed: ")
    else:
        filename = sys.argv[1]

    with open(filename, 'r') as f:
        module = f.read()

    bccEngine = BccEngine()
    bccEngine.execute(module)