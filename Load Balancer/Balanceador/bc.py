# command to disable RST flags when using RAW sockets: iptables -A OUTPUT -p tcp --tcp-flags RST RST -j DROP
# Usar iptables -D OUTPUT -p tcp --tcp-flags RST RST -j DROP
import argparse
import logging
import multiprocessing
import os
import socket
import struct
import sys
sys.path.insert(0, './Herramientas')
import threading
import time
from uuid import getnode as get_mac
import queue
import ipaddress


import bridge
import heartbeatDic
import net
import server_list

broadcast_address = "255.255.255.255"
HTTPSPORT = 443
BCPROTPORT = 2323
SERVPROTPORT = 12346
SERVCONF = 23456
PUBLICPORT = 443
CHECK_TIMEOUT = 5
CHECK_HEARTBEAT = 3
CHECK_MACS = 5
senderport = False

INTERFACE = "eno1"	#cambiar o calcular el que corresponda para la interfaz cableada

def listenClient(public_ip):
	if os.name == "nt":
		s = socket.socket(socket.AF_INET,socket.SOCK_RAW,socket.IPPROTO_IP)
		s.bind((public_ip,0))
		s.setsockopt(socket.IPPROTO_IP,socket.IP_HDRINCL,1)
		s.ioctl(socket.SIO_RCVALL,socket.RCVALL_ON)
	else:
		s=socket.socket(socket.PF_PACKET, socket.SOCK_RAW, socket.ntohs(0x0800))

	raw_s = socket.socket(socket.AF_INET, socket.SOCK_RAW, socket.IPPROTO_RAW)
	print("Bind to {}:{}".format(public_ip, PUBLICPORT))
	client_ip = None
	client_port = None
	while True:
		try:
			pkt=s.recvfrom(65565)
			for_me, client_ip = bridge.is_my_public_ip(pkt[0][14:34], public_ip)
			if for_me:
				new, client_port = bridge.is_new_connection(pkt[0][34:54], PUBLICPORT) 
				if new:
					logger.info("New connection from {}:{}".format(client_ip, client_port))
					# Enviar a servidor
					pool_lock.acquire()
					if servPool.isEmpty():
						logger.error("No available servers for client {}:{}".format(client_ip, client_port))
						pool_lock.release()
					else:
						server = servPool.getNext()
						pool_lock.release()
						logger.info("Client {}:{} assigned to server {}:{}".format(client_ip, client_port, server[0], server[1]))
						# Obtener puerto para conexión con el servidor
						fake = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
						fake.bind((public_ip, 0))
						binded_ip, private_port = fake.getsockname()
						fake.close()
						# Modificar paquete
						pkt2 = bridge.to_server_ip_change(pkt[0][14:34], private_ip, server[0]) + bridge.to_server_port_change(pkt[0][34:54], private_ip, private_port, server[0], server[1], pkt[0][54:]) + pkt[0][54:]
						# Disparar hilo de bridge
						eth_bridge = bridge.Eth_bridge()
						client_bridge = multiprocessing.Process(target=eth_bridge.run, args=(public_ip, private_ip, PUBLICPORT, private_port, client_ip, client_port, server[0], server[1]))
						client_bridge.start()
						raw_s.sendto(pkt2, (server[0], 0))

		except Exception as e:
			print(e)
			break

def serv_confirmation(data, sock, ip, port):
	code = bytes([2])
	data += 1
	msg = code + data.to_bytes(1, byteorder='big')
	sock.sendto(msg, (broadcast_address, SERVPROTPORT))
	#print("Confirmacion")
	logger.info("Confirmation sent to the server %s with port %s", ip, port)
	sock.sendto(msg, (broadcast_address, SERVCONF))

def bc_code3(sock, public_ip):
	code = bytes([3])
	msg = code + int(ipaddress.IPv4Address(public_ip)).to_bytes(4, byteorder='big')
	#msg = code + int(public_ip).to_bytes(32, byteorder='big')
	sock.sendto(msg, (broadcast_address, BCPROTPORT))

def bc_code4(sock, data):
	print("Quiero ser master")
	# La MAC se pasa como parametro
	code = bytes([4])
	msg = code + data.to_bytes(32, byteorder='big')
	sock.sendto(msg, (broadcast_address, BCPROTPORT))	

def i_am_master(sock):
	#print("Soy el nuevo master")
	# Opcode 5 para BC, 4 para Serv
	msg = bytes([5])
	sock.sendto(msg, (broadcast_address, BCPROTPORT))
	msg = bytes([4])
	sock.sendto(msg, (broadcast_address, SERVCONF))



def get_heartbeats(heartbeats):
	while True:
		silent = heartbeats.noHearbeat()

		ip = str(silent).translate(str.maketrans({"'":None}))
		ip2 = ip.translate(str.maketrans({"[":None}))
		ip_final = ip2.translate(str.maketrans({"]":None}))
		pool_lock.acquire()
		servPool.removeServer(ip_final)
		pool_lock.release()

		if silent:
			logger.info("Server with IP %s removed from the list of active servers", ip_final)
			
		time.sleep(CHECK_TIMEOUT)

def biggest_mac(a, b):
	if a > b:
		return a
	elif a < b:
		return b

def new_master(sockBC, my_mac):
	macs = []
	time_end = time.time() + 5

	try:
		sockBC.settimeout(time_end - time.time())
		while True:
			data, addr = sockBC.recvfrom(1024)
			#print("received message: ", data[0:1], data[1:])
			macs.append(int.from_bytes(data[1:], byteorder='big'))

	except socket.error as e:
		print(e)

	sockBC.settimeout(None)

	if len(macs) == 0:
		return True
	else:
		macs.append(my_mac)

		big = macs[0]
		for i in range(len(macs) - 1):
			big = biggest_mac(big, macs[i+1])

		print("New master: ", big)

		if big == my_mac:
			return True
		else:
			return False

def listenBC(broadcast_address, msg_queue, public_ip):
	sockBC = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
	sockBC.setsockopt(socket.SOL_SOCKET, socket.SO_BROADCAST, 1)
	sockBC.bind((broadcast_address, BCPROTPORT))
	wannabe = False
	active = True
	advice = False
	posibleMasters = []
	#numposibleMasters = 0

	my_mac = get_mac()

	print("Balancer Listener started")
	
	moment = time.time()

	while True:
		if active != True:
			if wannabe != True and (moment + 3 * CHECK_HEARTBEAT) <= time.time():
				wannabe = True
				time_end = time.time() + CHECK_MACS
				print("The last master died")
				logger.info("The previous master no longer exists")
				bc_code4(sockBC, my_mac)
				# Envia que quiere ser master
						
			if wannabe and time_end <= time.time():
				# Chequear lista quien va a ser el master
				if len(posibleMasters) == 1:
					active = True
				else:
					big = posibleMasters[0]
					for i in range(len(posibleMasters) - 1):
						big = biggest_mac(big, posibleMasters[i+1])
	
					print("New master: ", big)
	
					if big == my_mac:
						active = True
					else:
						active = False
						logger.info("Listen to active balancers")
				wannabe = False
				posibleMasters.clear()
				moment = time.time()
				#numposibleMasters = 0
	
			if active and advice != True:
				# Comienza a enviar hearbeat y avisa al main
				print("I am the new master")
				logger.info("I am the new master")
				msg_queue.put(public_ip)
				advice = True
				break
		
		sockBC.setblocking(0)
		try:
			data, addr = sockBC.recvfrom(1024)
			#print("received message: ", data)
			#print(data[0:1], data[1:])

			# Si quedaran tamaños fijos de paquetes se puede usar unpack para sacar sus valores más fácil
			op_code = int.from_bytes(data[0:1],byteorder='big')
			#payload = int.from_bytes(data[1:],byteorder='big') #ID o MAC addr
	
			if op_code == 3:
				#print ("Heartbeat")
				if active != True:
					# Sistema de 3 vidas
					moment = time.time()
					#sacar el public_ip
					temp = str(ipaddress.IPv4Address(int.from_bytes(data[1:],byteorder='big')))
					if temp != public_ip:
						public_ip = temp
					#print("llega heartbeat: ", public_ip)
	
			elif op_code == 4:
				identification = int.from_bytes(data[1:],byteorder='big')
				print(identification, " BC quiere ser master")
				if wannabe:
					posibleMasters.append(identification)
					# agrega a la pila
	
				if wannabe != True:
					posibleMasters.append(identification)
					wannabe = True
					bc_code4(sockBC, my_mac)
					logger.info("The previous master no longer exists")
					# Envia que quiere ser master
					time_end = time.time() + CHECK_MACS
	
			elif op_code == 5:
				pass

			elif op_code == 6:
				#print ("Info adicional")
				pass
	
			else:
				logger.error("Invalid Op Code") 
				#print("Op Code no válido")
		except:
			x = 0
			#logger.info("no message yet")

def listenServer(broadcast_address):
	sockS = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
	sockS.setsockopt(socket.SOL_SOCKET, socket.SO_BROADCAST, 1)
	sockS.bind((broadcast_address, SERVPROTPORT))
	print("Server Listener started")
	while True:
		data, addr = sockS.recvfrom(1024)
		#print("received message: ", data)
		op_code = int.from_bytes(data[0:1],byteorder='big')

		if op_code == 1:
			#print("Servidor {}:{} quiere agregarse a la red".format(addr[0], addr[1]))
			serv_port = HTTPSPORT
			if senderport:
				serv_port = int(addr[1])
			logger.info("Server {}:{} wants to join the network".format(addr[0], serv_port))
			identification = int.from_bytes(data[1:],byteorder='big')	
			pool_lock.acquire()
			added = servPool.addServer(addr[0], serv_port)
			#servPool.showPool()
			pool_lock.release()
			if added:
				logger.info("Server added to the network with IP %s and Port %s", addr[0], serv_port)            
				serv_confirmation(identification, sockS, addr[0], serv_port)

		elif op_code >= 2 and op_code <= 5:
			pass

		elif op_code == 6:
			print ("Info adicional")

		else:
			logger.error("Invalid Op Code")
			print("Op Code no válido")

def get_server_heartbeats(broadcast_address):
	sockS = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
	sockS.setsockopt(socket.SOL_SOCKET, socket.SO_BROADCAST, 1)
	sockS.bind((broadcast_address, SERVCONF))
	while True:
		try:
			data, addr = sockS.recvfrom(1024)
			#print("received message: ", data)
			op_code = int.from_bytes(data[0:1],byteorder='big')
			if op_code == 3:
				pool_lock.acquire()
				heartbeats[addr[0]] = time.time()
				pool_lock.release()
		except Exception as e:
			print(e)
			break

def send_heartbeats(broadcast_address, public_ip):
	sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
	sock.setsockopt(socket.SOL_SOCKET, socket.SO_BROADCAST, 1)
	while True:	
		try:
			bc_code3(sock, public_ip)
			time.sleep(5)
		except Exception as e:
			print(e)
			break

if __name__ == '__main__':
	parser = argparse.ArgumentParser(description='Load balancer. Requires sudo permissions')

	parser.add_argument("--publicinterface", "-pui", help="set public network interface, default lo")
	parser.add_argument("--privateinterface", "-pri", help="set private network interface, default eno1")
	parser.add_argument("--senderport","-sp", help="save server port as the port used to sent 'add me'. Port 443 as default", action="store_true")
	parser.add_argument("--raspbian", "-rb", help="running on raspbian OS. This changes the path of the log file", action="store_true")

	args = parser.parse_args()

	public = "lo"
	logfilename = "logEvents.log"
	# Change interfaces, if provided
	if args.publicinterface:
		public = args.publicinterface

	if args.privateinterface:
		INTERFACE = args.privateinterface

	if args.senderport:
		senderport = True

	if args.raspbian:
		logfilename = "/home/pi/Documents/proyecto-2-andromeda/Balanceador/logEvents.log"

    # Change private interface, if provided
	"""
	if len(sys.argv) >= 2:
		INTERFACE = sys.argv[1]
	if len(sys.argv) >= 3:
		public = sys.argv[2]"""

    #Create and configure logger 
	logging.basicConfig(filename=logfilename, format='%(asctime)s - %(name)s %(levelname)-8s %(message)s', filemode='w', datefmt='%a,  %d %b %Y %H:%M:%S',) 
  
    #Creating an object 
	logger=logging.getLogger() 
  
    #Setting the threshold of logger to DEBUG 
	logger.setLevel(logging.DEBUG) 

    # create logger
	logger = logging.getLogger('type')
	logger.setLevel(logging.DEBUG)

    # create console handler and set level to debug
	ch = logging.StreamHandler()
	ch.setLevel(logging.DEBUG)

    # create formatter
	formatter = logging.Formatter('%(asctime)s- %(name)s %(levelname)-8s %(message)s')

    # add formatter to ch
	ch.setFormatter(formatter)

	in_network = False
	active = True
	wannabe = False
	pool_lock = threading.Lock()
	servPool = server_list.ServerPool()
	heartbeats = heartbeatDic.Heartbeats()

	public_ip = net.get_ip_address(public)
	private_ip = net.get_ip_address(INTERFACE)
	
	broadcast = net.get_broadcast_ip(private_ip, net.get_netmask(INTERFACE))
	broadcast_address = str(broadcast)

	print("Public IP: ", public_ip)
	print("Private IP: {}\tBroadcast address: {}".format(private_ip, broadcast_address))
	
	# Calculando la red de la interfaz cableada
	mac = get_mac()
	#print (mac)
	msg_queue = queue.Queue()
	

	# Disparar thread BC-BC
	logger.info("Listen to active balancers")
	bcListen = threading.Thread(target=listenBC, args=[broadcast_address, msg_queue, public_ip])
	bcListen.start()

	
	while active != True:
		public_ip = msg_queue.get()
		active = True

	print("Master Public IP: ", public_ip)
	logger.info("Master Public IP: %s", public_ip)

	if active:
		# Empieza a escuchar servidores
		logger.info("Listen to active servers")
		servListen = threading.Thread(target=listenServer, args=[str(broadcast_address)])
		servListen.start()
		servHeart = threading.Thread(target=get_server_heartbeats, args=[str(broadcast_address)])
		servHeart.start()
		logger.info("Listen to active clients in {}:{}".format(public_ip, PUBLICPORT))
		clientListen = threading.Thread(target=listenClient, args=[str(public_ip)])
		clientListen.start()
		# Comenzar a revisar heartbeats de servidores
		heart = threading.Thread(target=get_heartbeats, args=[heartbeats])
		heart.start()
		# Cuando tiene todo listo, avisa que es master
		sockBC = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
		sockBC.setsockopt(socket.SOL_SOCKET, socket.SO_BROADCAST, 1)
		sockBC.bind((broadcast_address, BCPROTPORT))
		i_am_master(sockBC)
		# Empieza a enviar heartbeats
		alive = threading.Thread(target=send_heartbeats, args=[str(broadcast_address), public_ip])
		alive.start()
