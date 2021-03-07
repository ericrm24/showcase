SCOPE_TYPES = ['statement_if', 'for_f', 'while_f', 'function'] 

class FirstWalker():

    def __init__(self, ast):
        self.dictionary = ast

    def recursiveWalk(self, dictionary, parentId, deeperScope):
        identifier = parentId
        dic_type = dictionary['type']
        keys = dictionary.keys()

        if dic_type == 'declare_argument_list' or dic_type == 'argument_list':
            values = dictionary['values']
            if values != None:
                for value in values:
                    self.recursiveWalk(value, identifier, deeperScope)

        if dic_type in SCOPE_TYPES:
            #Abre un nuevo scope en la tabla de símbolos con identifier.
            deeperScope = deeperScope + 1
            identifier = deeperScope

        for key in keys:
            if isinstance(dictionary[key], dict):
                deeperScope = self.recursiveWalk(dictionary[key], identifier, deeperScope)

        dictionary['id'] = identifier

        return deeperScope

    def walk(self):
        self.recursiveWalk(self.dictionary, 0, 0)