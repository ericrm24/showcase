# ------------------------------------------------------------
 # Equipo: Elite Four
 # Integrantes: Ismael G., Diego N., Christian A., Eric R.
 #
 # Semantic Analyzer
 # ------------------------------------------------------------
import symbolTables
import inspect
from bccTypes import types

# Para mostrar el nombre de la función (si se quiere seguir el recorrido)
# 	print(inspect.getframeinfo(inspect.currentframe()).function)

class SemAnalyzer:

	def __init__(self, variables, functions, resolve = True):
		self.variables = variables
		self.functions = functions
		self.builtins = ['write', 'len', 'append', 'remove', 'readText', 'readNumber']
		self.warnings = []

	def startAnalysis(self, dictionary):
		self.statement_main(dictionary)
	
	def statement_main(self, dictionary):
		self.statements(dictionary['statements'])

	def block(self, dictionary):
		self.statements(dictionary['statements'])

	def statements(self, dictionary):
		if dictionary is not None:
			self.statement(dictionary['statement'])
			if dictionary.get("statements", False):
				self.statements(dictionary['statements'])

	def statement(self, dictionary):
		getattr(self, dictionary["type"])(dictionary)

	def assignment_statement(self, dictionary):
		#REVISAR SI SE USA [][] CON UNA MATRIZ O [] CON UNA LISTA
		self.variable_func(dictionary['variable'])
		valueType = self.value_func(dictionary['value'])
		varName = dictionary['variable']['name']
		result = self.variables.idLookup(dictionary['id'], varName)

		# Caso de si es un arreglo de forma a[index] o una matriz de forma a[index][indey]
		if "index" in dictionary:
			acceptedTypes = [types['INT'], types['UNDEFINED']]
			indexType = self.expression(dictionary['index'])
			indeyType = types['INT']
			if "indey" in dictionary:
				indeyType = self.expression(dictionary['indey'])
				if result[0] != types['MATRIX']:
					raise Exception('trying to use matrix[index][index] in something that isnt a matrix')
			elif result[0] != types['LIST']:
				raise Exception('trying to use array[index] in something that isnt an array')
			if indexType not in acceptedTypes or indeyType not in acceptedTypes:
				raise Exception('in array/matrix {} invalid type used as index'.format(varName))
			vectorType = result[2] 
			if vectorType != types["UNDEFINED"] and vectorType != valueType:
				raise Exception("Wrong type for already declared variable" )
		elif valueType != types["UNDEFINED"] and (result[0] != valueType and (result[0], result[2]) != valueType):
			raise Exception("Wrong type for already declared variable" )

	def declaration(self, dictionary):
		self.variable_func(dictionary['variable'])
		valueType = self.value_func(dictionary['value'])
		id = dictionary['id']
		varName = dictionary['variable']['name']
		result = self.variables.idLookup(id, varName)
		if result == None:
			raise Exception('Unexpected error: couldn\'t find the variable ' + varName)
		else:
			varType = result[0]
			if varType == types['UNDEFINED']:
				self.variables.idSetType(id, varName, valueType)
			elif varType != varType:
				raise Exception('The variable', varName, 'was already declared')

	def funccall_statement(self, dictionary):
		self.function_call(dictionary['funccall'])

	def function_call(self, dictionary):
		# En caso de ser función builtin
		if type(dictionary['functionname']) is dict:
			return self.builtin(dictionary)
		else:
			func_info = self.functions.lookup(dictionary["functionname"])
			identifier = func_info[3]['id']
			if not func_info:
				raise Exception("Function not found")
			argv = dictionary["arguments"]["values"]
			argc = 0
			if argv is not None:
				argc = len(argv)
			if argc != func_info[1]:
				raise Exception("Wrong number of arguments")
			func_type = func_info[0]
			arguments = dictionary['arguments']['values']
			arguments_type = [self.value_func(argument) for argument in arguments] if arguments != None else None
			arguments_name = func_info[2]
			if func_type == types['UNDEFINED']:
				func_body = func_info[3]
				func_type = self.resolve_function(func_body, arguments_type)
			else:
				if arguments_name != None:
					for index in range(len(arguments_name)):
						argument_type = self.variables.idLookup(identifier, arguments_name[index])[0]
						one_int_one_float = (argument_type == types['INT'] and arguments_type[index] == types['FLOAT']) or (argument_type == types['FLOAT'] and arguments_type[index] == types['INT'])
						if argument_type != arguments_type[index] and not one_int_one_float and arguments_type[index] != types['UNDEFINED']:
							raise Exception('Wrong type for argument ' + '\'' + arguments_name[index] + '\'' + ' in function call ' + '\'' + dictionary['functionname'] + '\', ' + 'the expected type was ' + str(argument_type) + ' and recieved type ' + str(arguments_type[index]))
			self.functions.addCall(dictionary["functionname"])
			return func_type

	def boolean_func(self, dictionary):
		if(dictionary['type'] == "boolean"):
			return types['BOOLEAN']
		else:
			raise Exception('Value wasn\'t boolean when boolean type was required')

	def int_func(self, dictionary):
		if(dictionary['type'] == "int"):
			return types['INT']
		else:
			raise Exception('Value wasn\'t an integer when integer type was required.')

	def float_func(self, dictionary):
		if(dictionary['type'] == "float"):
			return types['FLOAT']
		else:
			raise Exception('Value wasn\'t a float when float type was required.')

	def string_func(self, dictionary):
		if(dictionary['type'] == "string"):
			return types['STRING']
		else:
			raise Exception('Value wasn\'t a string when string type was required.')

	def expression(self, dictionary):
		if(dictionary['type'] == 'termexpr'):
			return self.term(dictionary['term'])
		if(dictionary['op'] in ['is', 'isnot', 'and', 'or']):
			return self.numeric_boolean_operands_expr(dictionary)
		else:
			return self.numeric_operands_expr(dictionary)	

	def numeric_operands_expr(self, dictionary):
		# casos de que la expresion sea expr [+,-,<,>,<=,>=] term
		operand1_type = self.expression(dictionary['expression'])
		operand2_type = self.term(dictionary['term'])
		accepted_types = [types['INT'], types['FLOAT'], types['STRING'], types['UNDEFINED']]
		if(operand1_type not in accepted_types):
			raise Exception('Invalid first operand')
		
		if(operand2_type not in accepted_types):
			raise Exception('Invalid second operand')

		if(operand1_type == types['UNDEFINED'] or operand2_type == types['UNDEFINED']):
			return types['UNDEFINED']

		if(dictionary['op'] in ['-','+']):
			if(operand1_type == types['BOOLEAN'] or operand2_type == types['BOOLEAN']):
				raise Exception('Can\'t use a boolean operand when operation is + or -')
			# String append
			elif(operand1_type == types['STRING'] or operand2_type == types['STRING']):
				if dictionary['op'] == '+':
					return types['STRING']
				else:
					raise Exception('Invalid operation for string')
			# Operaciones numéricas
			elif(operand1_type == types['INT'] and operand2_type == types['INT']):
				return types['INT']
			else:
				return types['FLOAT']
		else:
			return types['BOOLEAN']

	def numeric_boolean_operands_expr(self, dictionary):
		# casos de que la expresion sea expr [and,or,is,isnot] term
		operand1_type = self.expression(dictionary['expression'])
		operand2_type = self.term(dictionary['term'])
		op = dictionary['op']
		accepted_types = [types['INT'], types['FLOAT'], types['UNDEFINED'], types['BOOLEAN']]
	
		if(operand1_type not in accepted_types ):
			raise Exception('Invalid first operand')
		
		if(operand2_type not in accepted_types):
			raise Exception('Invalid second operand')

		if((operand1_type == types['BOOLEAN'] and (operand2_type != types['BOOLEAN'] and operand2_type != types['UNDEFINED'] )) or ((operand1_type != types['BOOLEAN'] and operand1_type != types['UNDEFINED']) and operand2_type == types['BOOLEAN'])):
			raise Exception('The operands are of an incompatible type')

		if((operand1_type not in [types['BOOLEAN'], types['UNDEFINED']]  or operand2_type not in [types['BOOLEAN'], types['UNDEFINED']]) and (op == 'and' or op == 'or')):
			raise Exception('A non boolean operand was given for a logic operator')

		if(operand1_type == types['UNDEFINED'] or operand2_type == types['UNDEFINED']):
			return types['UNDEFINED']

		return types['BOOLEAN']

	def variable_func(self, dictionary):
		result = None
		identifier = dictionary.get('id', None)
		if identifier != None:
			id = dictionary['id']
			result = self.variables.idLookup(id, dictionary['name'])
		if(result is None):
			raise Exception('Unknown variable ' + dictionary['name'] + ', not defined or incorrect type')
		else:
			variable_type = result[0]
			return variable_type

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

	def factor(self, dictionary):
		if(dictionary['type'] == 'primary'):
			return self.primary(dictionary['value'])
		elif(dictionary['op'] == 'not'):
			factor_type = self.factor(dictionary['factor'])
			if(factor_type != types['BOOLEAN'] and factor_type != types['UNDEFINED']):
				raise Exception('Incompatible operand')
			else:
				return factor_type
		else:
			factor_type = self.factor(dictionary['factor'])
			if(factor_type != types['INT'] and factor_type != types['FLOAT'] and factor_type != types['UNDEFINED']):
				raise Exception('Incompatible operand')
			else:
				return factor_type

	def term(self, dictionary):
		if(dictionary['type'] == 'factorterm'):
			return self.factor(dictionary['factor'])
		else:
			term_op1_type = self.term(dictionary['term'])
			term_op2_type = self.factor(dictionary['factor'])
			op = dictionary['op']
			#ACA SE PUEDE DETECTAR SI SE OCUPA PROMOVER O NO
			accepted_types = [types['UNDEFINED'], types['INT'], types['FLOAT']]
			if(term_op1_type not in accepted_types):
				raise Exception('Invalid first operand')
			if(term_op2_type not in accepted_types):
				raise Exception('Invalid second operand')
			if((term_op1_type != types['INT'] or term_op2_type != types['INT']) and op == '%'):
				raise Exception('Invalid operands for \% operator')
			
			if(term_op1_type == types['UNDEFINED'] or term_op2_type == types['UNDEFINED']):
				return types['UNDEFINED']
			if(term_op1_type == types['INT'] and term_op2_type == types['INT']):
				return types['INT']
			else:
				return types['FLOAT']


	def cond(self, dictionary):
		expression_type = self.expression(dictionary['condition'])
		if expression_type != types['BOOLEAN'] and expression_type != types['UNDEFINED']:
			raise Exception('The expression of the condition isn\'t equivalent to a truth value.')

	def statement_if(self, dictionary):
		self.cond(dictionary['cond'])
		self.block(dictionary['ifblock'])
		if dictionary.get('else', False):
			self.else_f(dictionary['else'])
		elif dictionary.get('elseifs', False):
			self.elseifs(dictionary['elseifs'])

	def else_f(self, dictionary):
		self.block(dictionary['elseblock'])

	def elseifs(self, dictionary):
		self.cond(dictionary['cond'])
		self.block(dictionary['elseifblock'])
		if dictionary['type'] == 'elseifelse':
			self.else_f(dictionary['else'])
		elif dictionary['type'] == 'elseifs':
			self.elseifs(dictionary['elseifs'])

	def for_f(self, dictionary):
		variable_type = self.variable_func(dictionary['variable'])
		if variable_type != types['INT']:
			raise Exception('The \'for\' variable isn\'t an int.')
		from_type = self.expression(dictionary['from'])
		to_type = self.expression(dictionary['to'])
		if from_type != types['INT'] and from_type != types['UNDEFINED']:
			raise Exception('The expression of the \'from\' of the \'for\' isn\'t an int.')
		if to_type != types['INT'] and to_type != types['UNDEFINED']:
			raise Exception('The expression of the \'to\' of the \'for\' isn\'t an int.')
		self.block(dictionary['forblock'])

	def while_f(self, dictionary):
		self.cond(dictionary['cond'])
		self.block(dictionary['whileblock'])

	def resolve_function(self, dictionary, arg_types):
		identifier = dictionary['id']
		var_name = dictionary['variable']['name']
		statements = dictionary['function_body']['statements']
		underAnalysis = self.functions.getUnderAnalysis(var_name)
		return_value_type = types['UNDEFINED']
		if underAnalysis:
			self.warnings.append('Warning: detected circular reference')
		else:
			self.functions.setUnderAnalysis(var_name, True)
			if arg_types != None:
				arguments = self.functions.lookup(var_name)[2]
				for index in range(len(arguments)):
					self.variables.idSetType(identifier, arguments[index], arg_types[index])
				self.statements(statements)
				return_value_type = self.function_block(dictionary['function_body'])
			else:
				return_value_type = self.function_block(dictionary['function_body'])
			self.functions.setUnderAnalysis(var_name, False)

		self.functions.defineType(var_name, return_value_type)
		return return_value_type

	def function_block(self, dictionary):
		self.statements(dictionary['statements'])
		return_value = types['VOID']
		if dictionary['retval']:
			return_value = self.value_func(dictionary['retval'])
		return return_value

	def builtin(self, dictionary):
		return getattr(self, 'builtin_' + dictionary['functionname']['name'])(dictionary['arguments'])

	def builtin_writeText(self, dictionary):
		argc = 0
		accepted_types = [types['STRING']]
		if dictionary.get('values', False):
			argc = len(dictionary['values'])

		if argc == 1:
			arg_type = self.value_func(dictionary['values'][0])
			if arg_type not in accepted_types:
				raise Exception('Argument type for \'write\' function is not valid.')
			else:
				return types['VOID']
		elif argc > 1:
			raise Exception('Too many arguments for \'write\' function. It takes 1.')
		else:
			raise Exception('Not enough arguments for \'write\' function. It takes 1.')

	def builtin_writeNumber(self, dictionary):
		argc = 0
		accepted_types = [types['INT']]
		if dictionary.get('values', False):
			argc = len(dictionary['values'])

		if argc == 1:
			arg_type = self.value_func(dictionary['values'][0])
			if arg_type not in accepted_types:
				raise Exception('Argument type for \'write\' function is not valid.')
			else:
				return types['VOID']
		elif argc > 1:
			raise Exception('Too many arguments for \'write\' function. It takes 1.')
		else:
			raise Exception('Not enough arguments for \'write\' function. It takes 1.')

	def builtin_readText(self, dictionary):
		argc = 0
		if dictionary.get('values', False):
			argc = len(dictionary['values'])
		if argc > 0:
			raise Exception('Too many arguments for \'readText\' function.')
		else:
			return types['STRING']

	def builtin_readNumber(self, dictionary):
		argc = 0
		if dictionary.get('values', False):
			argc = len(dictionary['values'])
		if argc > 0:
			raise Exception('Too many arguments for \'readNumber\' function.')
		else:
			# Puede cambiarse a INT si se vuelve necesario
			return types['INT']

	def builtin_len(self, dictionary):
		argc = 0
		if dictionary.get('values', False):
			argc = len(dictionary['values'])
		if argc == 1:
			arg_type = self.value_func(dictionary['values'][0])
			if arg_type != types['LIST'] and arg_type != types['UNDEFINED']:
				raise Exception('Argument for \'len\' function is not a list.')
			else:
				return types['INT']
		elif argc > 1:
			raise Exception('Too many arguments for \'len\' function. It takes 1.')
		else:
			raise Exception('Not enough arguments for \'len\' function. It takes 1.')

	def builtin_append(self, dictionary):
		argc = 0
		if dictionary.get('values', False):
			argc = len(dictionary['values'])
		if argc == 2:
			arg_types = self.arguments_builtin(dictionary)
			
			# Lo siguiente manejarlo según se haga argument_list

			# Revisar primer parámetro
			if arg_types[0] != types['LIST'] and arg_types[0] != types['UNDEFINED']:
				raise Exception('First argument for \'append\' function is not a list.')

			# Revisar segundo parámetro
			#if arg_types[1] != list_type and arg_types[1] != types['UNDEFINED']:
			#	raise Exception('Second argument for \'append\' function is not compatible with list type.')
			else:
				return types['VOID']
		elif argc > 2:
			raise Exception('Too many arguments for \'append\' function. It takes 2.')
		else:
			raise Exception('Not enough arguments for \'append\' function. It takes 2.')

	def builtin_remove(self, dictionary):
		argc = len(dictionary['values'])
		if argc == 2:
			arg_types = self.arguments_builtin(dictionary)

			# Lo siguiente manejarlo según se haga argument_list

			# Revisar primer parámetro
			if arg_types[0] != types['LIST'] and arg_types[0] != types['UNDEFINED']:
				raise Exception('First argument for \'remove\' function is not a list.')

			# Revisar segundo parámetro
			if arg_types[1] != types['INT'] and arg_types[1] != types['UNDEFINED']:
				raise Exception('Second argument for \'remove\' function is an integer.')
			else:
				return types['VOID']
		elif argc > 2:
			raise Exception('Too many arguments for \'remove\' function. It takes 2.')
		else:
			raise Exception('Not enough arguments for \'remove\' function. It takes 2.')

	def arguments_builtin(self, dictionary):
		# Para este punto se sabe que values no es None
		result = []
		for x in dictionary['values']:
			result.append(self.value_func(x))
		return result

	def value_list(self, dictionary):
		#checkear cada uno de los valores
		#si es value solo hay uno
		#si es values hay muchos
		types_in_list = []
		for x in dictionary:
			types_in_list.append(self.value_func(x))
		if(types_in_list[1:] == types_in_list[:-1]):
			if(type(types_in_list[0]) == tuple):
				return ( types['MATRIX'], types_in_list[0][1]) 
			else:
				return ( types['LIST'], types_in_list[0])
		else:
			raise Exception('List members must be of only one type')

	def value_func(self, dictionary):
		if(dictionary['type'] == 'value_list'):
			return self.value_list(dictionary['content'])
		elif(dictionary['type']=='expression_value'):
			return self.expression(dictionary['content'])
		else:
			raise Exception('Unknown value')

	def listsubs_func(self, dictionary):
		#checkear si expresion es un entero -index
		#checkear si la variable existe -variable
		#retornar el tipo
		expr_type = self.expression(dictionary['index'])
		id = dictionary['id']
		list_data = self.variables.idLookup(id, dictionary['variable']['name'])

		if(expr_type != types['INT']):
			raise Exception('Invalid index for a list, must be an int type')
		elif(list_data is None or list_data[0] != types['LIST']):
			raise Exception('Use of an undeclared list')
		return list_data[2]

	def value_expr(self, dictionary):
		return self.expression(dictionary['content'])

	def matrixsubs_func(self, dictionary):

		index_type = self.expression(dictionary['index'])
		indey_type = self.expression(dictionary['indey'])
		id = dictionary['id']
		list_data = self.variables.idLookup(id, dictionary['variable']['name'])

		if(index_type != types['INT'] and indey_type != types['INT']):
			raise Exception('Invalid index for a list, must be an int type')
		elif(list_data is None or list_data[0] != types['MATRIX']):
			raise Exception('Use of an undeclared Matrix')
		return list_data[2]