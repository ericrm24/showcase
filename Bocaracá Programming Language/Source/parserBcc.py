# ------------------------------------------------------------
 # Equipo: Elite Four
 # Integrantes: Ismael G., Diego N., Christian A., Eric R.
 #
 # Parser
 # ------------------------------------------------------------

import re
import ply.lex as lex
import ply.yacc as yacc
import lexerBcc as bocaracaLexer

#Import token list
tokens = bocaracaLexer.tokens

precedence = (
    ('left', 'PLUS', 'MINUS'),
    ('left', 'TIMES', 'DIVIDE'),
    ('left', 'NOT'),
    ('left', 'IF'), 
    ('left', 'ELSE'),
    ('left', 'ELSEIF')
 )

#-------------------AST root-------------------
def p_statement_main(p):
    '''program : statements ENDMARKER
               | funcdecls statements ENDMARKER'''
    if len(p) > 3:
        p[0] = {"type":"program", "funcdecls":p[1], "statements":p[2]}
    else:
        p[0] = {"type":"program", "statements":p[1]}

#-------------------general rules-------------------
def p_cond(p):
    'cond : LPAREN expression RPAREN COLON NEWLINE'
    p[0] = {"type": "cond", "condition": p[2]}

def p_block(p):
    'block : INDENT statements DEDENT'
    p[0] = {"type": "block", "statements":p[2]}

#-------------------statements-------------------
def p_statements(p):
    '''statements : statement
                  | statement statements'''
    if len(p) == 2:
        p[0] = {"type":"statement", "statement":p[1]}
    else :
        p[0] = {"type":"statements", "statement":p[1], "statements":p[2]}

def p_statement_declare(p):
    'statement : DECLARE variable ASSIGN value NEWLINE'
    p[0] = {"type":"declaration", "variable": p[2], "value":p[4]}

# VARDECSTATEMENT → varname = VALUE 
def p_statement_assign(p):
    '''statement : variable ASSIGN value NEWLINE
                 | variable LBRACKET expression RBRACKET ASSIGN value NEWLINE
                 | variable LBRACKET expression RBRACKET LBRACKET expression RBRACKET ASSIGN value NEWLINE'''
    if len(p) == 5:
        p[0] = {"type":"assignment_statement", "variable": p[1], "value":p[3]}
    elif len(p) == 8:
        # es una asignación de arreglo
        p[0] = {"type":"assignment_statement", "variable": p[1], "index": p[3], "value":p[6]}
    else:
        p[0] = {"type":"assignment_statement", "variable": p[1], "index": p[3], "indey":p[6], "value":p[9]}

def p_funccall_stmt(p):
    '''statement : funccall NEWLINE'''
    p[0] = {"type":"funccall_statement", "funccall":p[1]}

def p_function_call(p):
    '''funccall : builtin argument_list
                | VARIABLE_NAME argument_list'''
    p[0] = {"type": "funccall", "functionname":p[1], "arguments": p[2]}

#-------------------statements -> IF-------------------
def p_statement_if(p):
    '''statement : IF cond block
                 | IF cond block else
                 | IF cond block elseifs'''
    if len(p) == 4:
        p[0] = {"type":"statement_if", "cond": p[2], "ifblock": p[3]}
    elif len(p) == 5 and p[4]["type"] == "else":
        p[0] = {"type":"statement_if", "cond": p[2], "ifblock": p[3], "else":p[4]}
    else:
        p[0] = {"type":"statement_if", "cond": p[2], "ifblock": p[3], "elseifs": p[4]}

def p_else(p):
    'else : ELSE COLON NEWLINE block'
    p[0] = {"type": "else", "elseblock": p[4]}

def p_elseifs(p):
    '''elseifs : ELSEIF cond block
               | ELSEIF cond block else
               | ELSEIF cond block elseifs'''
    if (len(p) == 4):
        p[0] = {"type":"elseif", "cond":p[2], "elseifblock":p[3]}
    elif (len(p) == 5 and p[4]["type"] == "else"):
        p[0] = {"type":"elseifelse", "cond":p[2], "elseifblock":p[3], "else":p[4]}
    else:
        p[0] = {"type":"elseifs", "cond":p[2], "elseifblock":p[3], "elseifs":p[4]}

#-------------------statements -> FOR-------------------
def p_for(p):
    '''statement : FROM variable ASSIGN expression TO expression COLON NEWLINE block'''
    p[0] = {"type": "for_f", "variable": p[2], "from": p[4], "to": p[6], "forblock":p[9]}

#-------------------statements -> WHILE-------------------
def p_while(p):
    '''statement : WHILE cond block'''
    p[0] = {"type": "while_f", "cond": p[2], "whileblock":p[3]}

#-------------------types-------------------
def p_string(p):
    '''string : STRING'''
    p[0] = {"type":"string", "text":p[1]}
    
def p_int(p):
    '''int : INT'''
    p[0] = {"type":"int", "number":p[1]}

def p_float(p):
    '''float : FLOAT'''
    p[0] = {"type":"float", "number":p[1]}

# BOOL → true | false
def p_boolean(p):
    '''boolean : FALSE 
               | TRUE'''
    p[0] = {"type":"boolean", "state":p[1]}

def p_variable(p):
    '''variable : VARIABLE_NAME'''
    p[0] = {"type":"variable", "name":p[1]}

#-------------------list-------------------
def p_values(p):
    '''values : values COMMA value
              | value'''
    if len(p) == 2:
        p[0] = [p[1]]
    else:
        p[0] = p[1]
        p[0].append(p[3])

#-------------------expressions-------------------
def p_expression(p):
    '''expression : term
                  | expression PLUS term
                  | expression MINUS term
                  | expression AND term
                  | expression OR term
                  | expression LESS_THAN term
                  | expression GREATER_THAN term
                  | expression LESS_THAN_EQ term
                  | expression GREATER_THAN_EQ term
                  | expression IS term
                  | expression ISNOT term'''
    if len(p) == 2:
        p[0] = {"type":"termexpr", "term":p[1]}
    else:
        p[0] = {"type":"binopexpr", "expression":p[1], "op":p[2], "term":p[3]}

def p_term(p):
    '''term : factor
            | term TIMES factor
            | term DIVIDE factor
            | term MOD factor'''
    if len(p) == 2:
        p[0] = {"type":"factorterm", "factor":p[1]}
    else:
        p[0] = {"type":"binopterm", "term":p[1], "op":p[2], "factor":p[3]}

def p_factor(p):
    '''factor : primary
              | MINUS factor 
              | PLUS factor
              | NOT factor'''

    if len(p) == 2:
        p[0] = {"type":"primary", "value":p[1]}
    else:
        p[0] = {"type":"unopfactor", "op":p[1], "factor":p[2]}

def p_primary(p):
    '''primary : int
               | float
               | variable
               | boolean
               | string
               | funccall
               | listsubs
               | LPAREN expression RPAREN'''
    if len(p) == 2:
        p[0] = {"type":"literal", "value":p[1]}
    else:
        p[0] = {"type":"parenexpr", "expression":p[2]}


#-------------------list index-------------------
def p_value_list_subs(p):
    '''listsubs : variable LBRACKET expression RBRACKET
                | variable LBRACKET expression RBRACKET LBRACKET expression RBRACKET'''

    if len(p) == 5:
        p[0] = {"type":"listsubs", "variable": p[1], "index":p[3]}
    else:
        p[0] = {"type":"matrixsubs", "variable": p[1], "index":p[3], "indey":p[6]}

#-------------------functions-------------------

def p_funcdecls(p):
    '''funcdecls : funcdecl
                  | funcdecl funcdecls'''
    if len(p) == 2:
        p[0] = {"type":"funcdecl", "funcdecl":p[1]}
    else :
        p[0] = {"type":"funcdecls", "funcdecl":p[1], "funcdecls":p[2]}

def p_funcdecl(p):
    '''funcdecl : FUNCTION variable declare_argument_list COLON NEWLINE funcblock'''
    p[0] = {"type": "function", "variable": p[2], "arguments":p[3], "function_body": p[6]}

def p_arguments(p):
    '''arguments : arguments COMMA variable
                 | variable'''
    if len(p) == 2:
        p[0] = [p[1]]
    else:
        p[0] = p[1]
        p[0].append(p[3])

def p_declare_argument_list(p):
    '''declare_argument_list : LPAREN arguments RPAREN
                             | LPAREN RPAREN'''
    if len(p) == 4:
        p[0] = {"type": "declare_argument_list", "values":p[2]}
    else:
        p[0] = {"type": "declare_argument_list", "values":None}

def p_argument_list(p):
    '''argument_list : LPAREN values RPAREN
                     | LPAREN RPAREN'''
    if len(p) == 4:
        p[0] = {"type": "argument_list", "values":p[2]}
    else:
        p[0] = {"type": "argument_list", "values":None}

def p_function_block(p):
    '''funcblock : INDENT statements DEDENT
                 | INDENT RETURN value NEWLINE DEDENT
                 | INDENT statements RETURN value NEWLINE DEDENT'''
    retval = None
    statements = p[2]
    if len(p) == 7:
        retval = p[4]
    elif len(p) == 6:
        retval = p[3]
        statements = None
    p[0] = {"type": "funcblock", "statements": statements , "retval":retval}

def p_builtin(p):
    '''builtin : WRITETEXT
               | WRITENUMBER
               | READTEXT
               | READNUMBER
               | LEN
               | APPEND
               | REMOVE'''
    p[0] = {"type": "builtin", "name": p[1]}

#-------------------other-------------------
# VALUE → number | string | varname | EXPR | FUNCALL | BOOL
def p_value_list(p):
    '''value : LBRACKET values RBRACKET'''
    p[0] = {"type": "value_list", "content": p[2]}

def p_value_expression(p):
    'value : expression'
    p[0] = {"type":'expression_value', "content":p[1]}

#-------------------Parser error-------------------
def p_error(p):
    if p:
          print("Syntax error at token ", p.value, " of type ", p.type, " at line ", p.lineno, sep = '')
          raise Exception("Syntax error at token ", p.value, " of type ", p.type, " at line ", p.lineno, sep = '')
    else:
          print("Syntax error at EOF")
          raise Exception("Syntax error at EOF")


class BocaracaParser(object):

    def __init__(self, lexer=None, show_tokens=True, show_ast=True):
        if lexer is None:
            lexer = bocaracaLexer.IndentLexer()
        self.lexer = lexer
        self.parser = yacc.yacc()
        self.print_tokens = show_tokens
        self.print_ast = show_ast
        self.error = False

    def parse(self, code):
        self.lexer.input(code)
        if self.print_tokens:
            self.show_tokens()
            self.lexer.input(code)
        result = None
        try:
            result = self.parser.parse(lexer=self.lexer)
        except:
            self.print_ast = False
            self.error = True
        self.ast = result
        if self.print_ast:
            self.show_ast()
        return result
    
    def show_tokens(self):
        print("Tokens")
        while True:
            token = self.lexer.token()
            if not token:
                break      # No more input
            if (re.match(r'((\r)?\n)+', str(token.value)) is None):
                print("    " + str(token.value) + " -> " + token.type)
            else:
                print("    " + str("\\n") + " -> " + token.type)
        print("\n")
    
    def show_ast(self, ast = None, lvl = 1):
        print("Abstract Syntax Tree")
        if ast == None:
            self.show_ast_r(self.ast, lvl)
        else:
            self.show_ast_r(ast, lvl)
        print("\n")

    def show_ast_r(self, node, level):
        if 'id' in node:
            for _ in range(level):
                print("|    ", end = '')
            print('id', ": ", node['id'], sep = '')
        for key in node:
            if not isinstance(node[key],dict) and not isinstance(node[key],list):
                if key != 'id':
                    for _ in range(level):
                        print("|    ", end = '')
                    print(key, ": ", node[key], sep = '')
            elif not isinstance(node[key],list):
                for _ in range(level):
                    print("|    ", end = '')
                print(key, ": ", sep = '')
                self.show_ast_r(node[key],level+1)
            else:
                for _ in range(level + 1):
                    print("|    ", end = '')
                print(key, ": ", node[key], sep = '')
