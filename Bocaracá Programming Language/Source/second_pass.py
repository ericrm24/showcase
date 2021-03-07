from semAnalyzerBcc import SemAnalyzer
from bccTypes import types

SCOPE_TYPES = ['statement_if', 'for_f', 'while_f', 'function'] 

class SecondWalker():

    def __init__(self, ast, variables, functions):
        self.dictionary = ast
        self.variables = variables
        self.functions = functions
        self.semAnalyzer = SemAnalyzer(variables, functions)

    def recursiveWalk(self, dictionary):
        identifier = dictionary['id']
        new_scope = False
        dic_type = dictionary['type']
        var_name = ''
        keys = dictionary.keys()
        # Si es asignación, asegurarse de que antes se declaró
        if dic_type == 'assignment_statement':
            var_name = dictionary['variable']['name']
            self.variableCheck(var_name, "Value assigned to variable " + var_name +  " before declaration")
        # Si se usa dentro de una expresión, asegurarse de que se ha declarado
        elif dic_type == 'expression_value':
            self.existsInExpression(dictionary['content'])
        # Si se usa como parámetro de llamada a función, revisar que existe
        elif dic_type == 'funccall':
            self.existsInFunccall(dictionary['arguments'])
        elif dic_type == 'declaration':
            #Inserta en la tabla de símbolos.
            var_name = dictionary['variable']['name']
            self.semAnalyzer.variables = self.variables
            self.semAnalyzer.functions = self.functions
            if dictionary['value']['type'] != 'value_list':
                self.variables.insert(var_name, types['UNDEFINED'])
            else:
                var_type = self.semAnalyzer.value_func(dictionary['value'])
                self.variables.insertList(var_name, var_type[0], var_type[1])
        elif dic_type in SCOPE_TYPES:
            #Abre un nuevo scope en la tabla de símbolos con identifier.
            self.variables.initializeScope(identifier)
            new_scope = True
            if dic_type == 'for_f':
                #Inserta la variable contadora del for en la tabla de símbolos.
                var_name = dictionary['variable']['name']
                self.variables.insert(var_name, types['INT'])
            if dic_type == 'function':
                #Inserta la función en la tabla de símbolos.
                var_name = dictionary['variable']['name']
                arguments = dictionary['arguments']['values']
                # Asegurarse de que tiene argumentos
                if arguments:
                    names = [argument['name'] for argument in arguments]
                    self.functions.insert(var_name, types['UNDEFINED'], len(names), names)
                    for name in names:
                        self.variables.insert(name, types['UNDEFINED'])
                else:
                    self.functions.insert(var_name, types['UNDEFINED'], 0, None)
                self.functions.saveBody(var_name, dictionary)

        for key in keys:
            if isinstance(dictionary[key], dict):
                deeperScope = self.recursiveWalk(dictionary[key])

        if new_scope:
            #Cierra el scope en la tabla de símbolos.
            self.variables.finalizeScope()

    def walk(self):
        self.recursiveWalk(self.dictionary)
    
    def existsInFunccall(self, dictionary):
        if dictionary.get('values', False):
            arguments = dictionary['values']
            for argument in arguments:
                if argument.get('content', False):
                    self.existsInExpression(argument['content'])
    
    def existsInExpression(self, dictionary):
        if dictionary.get('type', False):
            # Expression
            if dictionary['type'] == 'binopexpr':
                self.existsInExpression(dictionary['expression'])
            # Term
            self.termCheck(dictionary['term'])
    
    def termCheck(self, dictionary):
        # Obtener diccionario del factor
        dict_type = dictionary['type']
        # Term
        if dict_type == 'binopterm':
            self.termCheck(dictionary['term'])
        # Factor
        self.factorCheck(dictionary['factor'])
    
    def factorCheck(self, dictionary):
        dict_type = dictionary['type']
        # Primary
        if dict_type == 'primary':
            prim_dict = dictionary['value']
            if prim_dict.get('type', False):
                # Literal
                if prim_dict['type'] == 'literal':
                    self.literalCheck(prim_dict['value'])
                # Paren Expr
                elif prim_dict['type'] == 'parenexpr':
                    self.existsInExpression(prim_dict['expression'])
        elif dict_type == 'unopfactor':
            self.factorCheck(dictionary['factor'])
        else:
            raise Exception('Incorrect factor type')
    
    def literalCheck(self, dictionary):
        if dictionary['type'] == 'variable':
            var_name = dictionary['name']
            self.variableCheck(var_name, "Variable " + var_name + " used before declaration")

    def variableCheck(self, var_name, msg):
        variable = self.variables.lookup(var_name)
        if variable is None:
            raise Exception(msg)