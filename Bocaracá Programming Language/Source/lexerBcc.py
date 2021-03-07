# ------------------------------------------------------------
 # Equipo: Elite Four
 # Integrantes: Ismael G., Diego N., Christian A., Eric R.
 #
 # Lexer
 # ------------------------------------------------------------

import ply.lex as lex

def appendByKey(dictonary, key, value):
  dictonary[key].insert(0,value)
  return dictonary

# Reserved keywords
keywords = (
    'IS',
    'ISNOT',
    'OR',
    'AND',
    'NOT',
    'IF',
    'ELSE',
    'ELSEIF',
    'FROM',
    'TO',
    'WHILE',
    'FUNCTION',
    'RETURN',
    'TRUE',
    'FALSE',
    'LEN',
    'APPEND',
    'REMOVE',
    'DECLARE'
)

keyword_map = {
    'writeText' : 'WRITETEXT',
    'writeNumber' : 'WRITENUMBER',
    'readText' : 'READTEXT',
    'readNumber' : 'READNUMBER',
}
for keyword in keywords:
    keyword_map[keyword.lower()] = keyword

# List of token names
tokens = [
    'VARIABLE_NAME',
    'COMMA',
    'COLON',
    'INT',
    'FLOAT',
    'PLUS',
    'MINUS',
    'TIMES',
    'DIVIDE',
    'MOD',
    'ASSIGN',
    'LESS_THAN',
    'GREATER_THAN',
    'LESS_THAN_EQ',
    'GREATER_THAN_EQ',
    'LPAREN',
    'RPAREN',
    'LBRACKET',
    'RBRACKET',
    'NEWLINE',
    'INDENT',
    'DEDENT',
    'WS',
    'STRING',
    'ENDMARKER'
] + list(keyword_map.values())
 
# Regular expression rules for simple tokens
t_COMMA = r','
t_COLON = r':'
t_PLUS    = r'\+'
t_MINUS   = r'-'
t_TIMES   = r'\*'
t_DIVIDE  = r'/'
t_MOD = r'%'
t_ASSIGN = r'='
t_LESS_THAN = r'<'
t_GREATER_THAN = r'>'
t_LESS_THAN_EQ = r'<='
t_GREATER_THAN_EQ = r'>='
t_LBRACKET = r'\['
t_RBRACKET = r'\]'

# String -> matches anything between two "
#        -> uses lookbehind for \" cases, except when \\ precedes
def t_STRING(t):
    r'(?:\"(([^\"])|((?<=\\)(?<!\\\\)\"))*\"|\'(([^\'])|((?<=\\)(?<!\\\\)\'))*\')'
    return t

def t_FLOAT(t):
    r'\d+\.\d+'
    t.value = float(t.value)
    return t

def t_INT(t):
    r'\d+'
    t.value = int(t.value)    
    return t

# Identifiers
def t_VARIABLE_NAME(t):
    r'_?[a-zA-Z][a-zA-Z0-9_]*'
    t.type = keyword_map.get(t.value, 'VARIABLE_NAME')
    return t

# Putting this before t_WS let it consume lines with only comments in
# them so the latter code never sees the WS part.  Not consuming the
# newline.  Needed for "if 1: #comment"
def t_COMMENT(t):
    r"[ ]*\043[^\n]*"  # \043 is '#'
    pass

# Whitespace
def t_WS(t):
    r'[ \t]+'
    if t.lexer.at_line_start and t.lexer.paren_count == 0:
        return t

def t_newline(t):
    r'((\r)?\n)+'
    t.lexer.lineno += len(t.value)
    t.type = "NEWLINE"
    if t.lexer.paren_count == 0:
        return t


# Parenthesis modify paren_count
def t_LPAREN(t):
    r'\('
    t.lexer.paren_count += 1
    return t

def t_RPAREN(t):
    r'\)'
    t.lexer.paren_count -= 1
    return t

# Error handling rule
def t_error(t):
    print("Error léxico '%s'" % t.value[0])
    t.lexer.skip(1)

# INDENT / DEDENT generation as a post-processing filter

# The original lex token stream contains WS and NEWLINE characters.
# WS will only occur before any other tokens on a line.

# I have three filters.  One tags tokens by adding two attributes.
# "must_indent" is True if the token must be indented from the
# previous code.  The other is "at_line_start" which is True for WS
# and the first non-WS/non-NEWLINE on a line.  It flags the check so
# see if the new line has changed indication level.

# Python's syntax has three INDENT states
#  0) no colon hence no need to indent
#  1) "if 1: go()" - simple statements have a COLON but no need for an indent
#  2) "if 1:\n  go()" - complex statements have a COLON NEWLINE and must indent
NO_INDENT = 0
MAY_INDENT = 1
MUST_INDENT = 2

# Only care about whitespace at the start of a line
def track_tokens_filter(lexer, tokens):
    lexer.at_line_start = at_line_start = True
    indent = NO_INDENT
    saw_colon = False
    for token in tokens:
        token.at_line_start = at_line_start

        if token.type == "COLON":
            at_line_start = False
            indent = MAY_INDENT
            token.must_indent = False

        elif token.type == "NEWLINE":
            at_line_start = True
            if indent == MAY_INDENT:
                indent = MUST_INDENT
            token.must_indent = False

        elif token.type == "WS":
            assert token.at_line_start == True
            at_line_start = True
            token.must_indent = False

        else:
            # A real token; only indent after COLON NEWLINE
            if indent == MUST_INDENT:
                token.must_indent = True
            else:
                token.must_indent = False
            at_line_start = False
            indent = NO_INDENT

        yield token
        lexer.at_line_start = at_line_start


def _new_token(type, lineno):
    tok = lex.LexToken()
    tok.type = type
    tok.value = None
    tok.lineno = lineno
    return tok

# Synthesize a DEDENT tag
def DEDENT(lineno):
    return _new_token("DEDENT", lineno)

# Synthesize an INDENT tag
def INDENT(lineno):
    return _new_token("INDENT", lineno)


# Track the indentation level and emit the right INDENT / DEDENT events.
def indentation_filter(tokens):
    # A stack of indentation levels; will never pop item 0
    levels = [0]
    token = None
    depth = 0
    prev_was_ws = False
    prev_was_nl = False
    for token in tokens:
        # WS only occurs at the start of the line
        # There may be WS followed by NEWLINE so
        # only track the depth here.  Don't indent/dedent
        # until there's something real.
        if token.type == "WS":
            assert depth == 0
            depth = len(token.value)
            prev_was_ws = True
            # WS tokens are never passed to the parser
            continue

        if token.type == "NEWLINE":
            depth = 0
            if prev_was_ws or token.at_line_start:
                # ignore blank lines
                continue
            # pass the other cases on through
            prev_was_nl = True
            yield token
            continue

        # then it must be a real token (not WS, not NEWLINE)
        # which can affect the indentation level

        prev_was_ws = False
        prev_was_nl = False
        if token.must_indent:
            # The current depth must be larger than the previous level
            if not (depth > levels[-1]):
                raise IndentationError("expected an indented block")

            levels.append(depth)
            yield INDENT(token.lineno)

        elif token.at_line_start:
            # Must be on the same level or one of the previous levels
            if depth == levels[-1]:
                # At the same level
                pass
            elif depth > levels[-1]:
                raise IndentationError(
                    "indentation increase but not in new block")
            else:
                # Back up; but only if it matches a previous level
                try:
                    i = levels.index(depth)
                except ValueError:
                    raise IndentationError("inconsistent indentation")
                for _ in range(i + 1, len(levels)):
                    yield DEDENT(token.lineno)
                    levels.pop()

        yield token

    ### Finished processing ###

    # Include NEWLINE if needed
    if not prev_was_nl:
        tok = lex.LexToken()
        tok.type = "NEWLINE"
        tok.value = "\n"
        tok.lineno = token.lineno
        yield tok

    # Must dedent any remaining levels
    if len(levels) > 1:
        assert token is not None
        for _ in range(1, len(levels)):
            yield DEDENT(token.lineno)


# The top-level filter adds an ENDMARKER, if requested.
def filter(lexer, add_endmarker=True):
    token = None
    tokens = iter(lexer.token, None)
    tokens = track_tokens_filter(lexer, tokens)
    for token in indentation_filter(tokens):
        yield token

    if add_endmarker:
        lineno = 1
        if token is not None:
            lineno = token.lineno
        yield _new_token("ENDMARKER", lineno)

# Combine Ply and own filters into a new lexer
class IndentLexer(object):

    def __init__(self, debug=0, optimize=0, lextab='lextab', reflags=0):
        self.lexer = lex.lex(debug=debug, optimize=optimize,
                             lextab=lextab, reflags=reflags)
        self.token_stream = None

    def input(self, s, add_endmarker=True):
        self.lexer.paren_count = 0
        self.lexer.input(s)
        self.token_stream = filter(self.lexer, add_endmarker)

    def token(self):
        try:
            return next(self.token_stream)
        except StopIteration:
            return None