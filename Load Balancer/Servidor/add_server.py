import argparse
import http.server
import os
import random
import socket
import socketserver
import struct
import sys
sys.path.insert(0, '../Balanceador/Herramientas')
import threading
import time

import net

PORT = 8000
BCPORT = 12346
CONFPORT = 23456
INTERFACE = "eno1"
IP = "127.0.0.1"
Handler = http.server.SimpleHTTPRequestHandler
bc_ip = None
BROADCAST = "255.255.255.255"
BEAT_PERIOD = 2

def add_to_network():
	# Generar identificación
	rand = random.randint(0,255)
	msg = bytes([1]) + rand.to_bytes(1, byteorder='big')
	confirmed = False

	sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
	sock.setsockopt(socket.SOL_SOCKET, socket.SO_BROADCAST,1)
	sock.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
	sockC = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
	sockC.setsockopt(socket.SOL_SOCKET, socket.SO_BROADCAST,1)
	sockC.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)

	sock.bind((IP, PORT))
	sockC.bind((BROADCAST, CONFPORT))
	sockC.setblocking(0)

	# Ciclo para reintentar
	while confirmed == False:
		# Para usar el mismo puerto que utiliza el servidor para escuchar
		sock.sendto(msg, (BROADCAST, CONFPORT))
		wait_t = time.time() + 5
		# Recibir confirmación de broadcast
		while time.time() < wait_t and confirmed == False:
			try:
				data, addr = sockC.recvfrom(1024)
				op_code = int.from_bytes(data[0:1],byteorder='big')
				if op_code == 2:
					confirmation = int.from_bytes(data[1:],byteorder='big')
					if confirmation == rand + 1:
						confirmed = True
						sock.close()
						sockC.close()
						print("Success adding to network!")
			except:
				x = 0

def wait_new_master():
	sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
	sock.setsockopt(socket.SOL_SOCKET, socket.SO_BROADCAST,1)
	sock.bind((BROADCAST, BCPORT))

	while True:
		try:
			data, addr = sock.recvfrom(1024)
			op_code = int.from_bytes(data[0:1],byteorder='big')
			if op_code == 4:
				os.execl(sys.executable, sys.executable, *sys.argv)

		except Exception as e:
			print(e)
			break


def send_heartbeat():
	
	msg = bytes([3])

	sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
	sock.setsockopt(socket.SOL_SOCKET, socket.SO_BROADCAST,1)

	# Ciclo para reintentar
	# Para usar el mismo puerto que utiliza el servidor para escuchar
	sock.bind((IP, 0))
	
	while True:
		try:
			sock.sendto(msg, (BROADCAST, BCPORT))
			time.sleep(BEAT_PERIOD)
		except Exception as e:
			print(e)
			break

if __name__ == '__main__':

	parser = argparse.ArgumentParser()

	parser.add_argument("--port", "-p", help="set port to send 'add me', default 8000")
	parser.add_argument("--interface", "-i", help="set network interface, default eno1")

	args = parser.parse_args()

	if args.port:
		PORT = int(args.port)

	if args.interface:
		INTERFACE = args.interface

	IP = net.get_ip_address(INTERFACE)
	broadcast_addr = net.get_broadcast_ip(IP, net.get_netmask(INTERFACE))
	BROADCAST = str(broadcast_addr)
	print("Server IP: {}\tBroadcast address: {}".format(IP, BROADCAST))

	add_to_network()
	
	heart = threading.Thread(target=send_heartbeat, args=())
	heart.start()

	new_master = threading.Thread(target=wait_new_master, args=())
	new_master.start()

	"""with socketserver.TCPServer(("", PORT), Handler) as httpd:
		print("serving at port", PORT)
		try:
			httpd.serve_forever()
		except KeyboardInterrupt:
			pass
		finally:
			# Clean-up server (close socket, etc.)
			httpd.shutdown()"""
