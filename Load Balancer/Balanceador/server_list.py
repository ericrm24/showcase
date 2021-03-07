#Comando para ejecutar: python3 serverList.py data.csv

from typing import NamedTuple
from itertools import cycle
import sys
import csv

class ServerInfo(NamedTuple):
	ip: str
	port: int

class ServerPool():

	def __init__(self):
		self.serverList = []
		self.listSize = 0
		self.index = 0

	# Si definitivamente nunca van a repetirse, se puede quitar la búsqueda y simplemente agregarlo
	def addServer(self, ip, port):
		added = False
		newServer = ServerInfo(ip, int(port))
		if newServer in self.serverList:
			print("in list")
		else:
			self.serverList.append(newServer)
			self.listSize += 1
			added = True
		return added

	def showPool(self):
		for i in self.serverList:
			print (i)

	def removeServer(self, ip):
		self.serverList = [x for x in self.serverList if x.ip != ip]
		self.listSize = len(self.serverList)

	def getNext(self):
		next = self.serverList[self.index]
		self.index = 0 if self.index + 1 == self.listSize else self.index + 1
		return next

	def isEmpty(self):
		return self.listSize == 0