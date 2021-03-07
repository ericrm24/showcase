import logging
import socket
import struct
import binascii
import os
import threading
import sys

import pack_analysis

class Eth_bridge():

	# filters packets to Load Balancer
	# data sent should be pkt[0][14:34]
	def is_my_ip(self, data, fromClient=False):
		storeobj = struct.unpack("!BBHHHBBH4s4s", data)
		_destination_address = socket.inet_ntoa(storeobj[9])

		if fromClient:
			return _destination_address == self.public_ip
		else:
			return _destination_address == self.private_ip

	# filters packets to Load Balancer's client/server ports
	# data sent should be pkt[0][34:54]
	def is_port(self, data, fromClient=False):
		storeobj = struct.unpack('!HHLLBBHHH',data)
		_destination_port = storeobj[1]
		_tcp_flag  =storeobj[5]

		if fromClient:
			if _destination_port == self.my_port_c and _tcp_flag  & 2 != 0:		# SYN flag
				self.client_port = storeobj[0]		# store the client port contained in the packet
			return _destination_port == self.my_port_c
		else:
			return _destination_port == self.my_port_s

	# filters packets to client/server IP
	# data sent should be pkt[0][14:34]
	def is_ip(self, data, fromClient=False):
		storeobj = struct.unpack("!BBHHHBBH4s4s", data)
		_source_address =socket.inet_ntoa(storeobj[8])

		if fromClient:
			return _source_address == self.client_host
		else:
			return _source_address == self.server_host

	def to_client_ip(self, data):
		storeobj = struct.unpack("!BBHHHBBH4s4s", data)
		_source_address = socket.inet_aton(self.public_ip)
		_destination_address = socket.inet_aton(self.client_host)
		_header_checksum = struct.pack("!BBHHHBBH4s4s", storeobj[0], storeobj[1], storeobj[2], storeobj[3], storeobj[4], storeobj[5], storeobj[6], 0, _source_address, _destination_address)
		_header = struct.pack("!BBHHHBBH4s4s", storeobj[0], storeobj[1], storeobj[2], storeobj[3], storeobj[4], storeobj[5], storeobj[6], checksum(bytearray(_header_checksum), len(_header_checksum)), _source_address, _destination_address)
		return _header

	def to_client_port(self, data, payload):
		storeobj = struct.unpack('!HHLLBBHHH',data)
		_source_port = self.my_port_c
		_destination_port = self.client_port
		raw = struct.pack("!HHLLBBHHH", _source_port, _destination_port, storeobj[2], storeobj[3], storeobj[4], storeobj[5], storeobj[6], 0, storeobj[8])
		
		# Check if the connection is ending
		_tcp_flag  =storeobj[5]
		if _tcp_flag & 16 != 0 and _tcp_flag & 1 != 0:	# FIN ACK
			self.end2 = True

		# Get data for checksum
		_source_address = socket.inet_aton(self.public_ip)
		_destination_address = socket.inet_aton(self.client_host)
		placeholder = 0
		protocol = socket.IPPROTO_TCP
		tcp_len = len(raw) + len(payload)
		# Assemble pseudo header
		psh = struct.pack('!4s4sBBH', _source_address, _destination_address, placeholder, protocol, tcp_len)
		psh = psh + raw + payload
		# Create header with new checksum
		_header = struct.pack("!HHLLBBHHH", _source_port, _destination_port, storeobj[2], storeobj[3], storeobj[4], storeobj[5], storeobj[6], checksum(bytearray(psh), len(psh)), storeobj[8])
		return _header

	def to_server_ip(self, data):
		storeobj = struct.unpack("!BBHHHBBH4s4s", data)
		_source_address = socket.inet_aton(self.private_ip)
		_destination_address = socket.inet_aton(self.server_host)
		_header_checksum = struct.pack("!BBHHHBBH4s4s", storeobj[0], storeobj[1], storeobj[2], storeobj[3], storeobj[4], storeobj[5], storeobj[6], 0, _source_address, _destination_address)
		_header = struct.pack("!BBHHHBBH4s4s", storeobj[0], storeobj[1], storeobj[2], storeobj[3], storeobj[4], storeobj[5], storeobj[6], checksum(bytearray(_header_checksum), len(_header_checksum)), _source_address, _destination_address)
		return _header

	def to_server_port(self, data, payload):
		storeobj = struct.unpack('!HHLLBBHHH',data)
		_source_port = self.my_port_s
		_destination_port = self.server_port
		raw = struct.pack("!HHLLBBHHH", _source_port, _destination_port, storeobj[2], storeobj[3], storeobj[4], storeobj[5], storeobj[6], 0, storeobj[8])
		
		# Check if the connection is ending
		_tcp_flag  =storeobj[5]
		if _tcp_flag & 16 != 0 and _tcp_flag & 1 != 0:		# FIN ACK
			self.end1 = True

		# Get data for checksum
		_source_address = socket.inet_aton(self.private_ip)
		_destination_address = socket.inet_aton(self.server_host)
		placeholder = 0
		protocol = socket.IPPROTO_TCP
		tcp_len = len(raw) + len(payload)
		# Assemble pseudo header
		psh = struct.pack('!4s4sBBH', _source_address, _destination_address, placeholder, protocol, tcp_len)
		psh = psh + raw + payload
		# Create header with new checksum
		_header = struct.pack("!HHLLBBHHH", _source_port, _destination_port, storeobj[2], storeobj[3], storeobj[4], storeobj[5], storeobj[6], checksum(bytearray(psh), len(psh)), storeobj[8])
		return _header

	def check_change(self, pkt):
		unpack=pack_analysis.unpack()
		print ("\n===>> [+] ------------ IP Header ------------[+]")
		for i in unpack.ip_header(pkt[0:20]).items():
			a,b=i
			print ("{} : {} | ".format(a,b))
		print ("\n===>> [+] ------------ Tcp Header ----------- [+]")
		for  i in unpack.tcp_header(pkt[20:40]).items():
			a,b=i
			print ("{} : {} | ".format(a,b))

	def run(self, public_ip, private_ip, my_port_c, my_port_s, client_host, client_port, server_host, server_port):
		self.public_ip = public_ip
		self.private_ip = private_ip
		self.my_port_c = my_port_c
		self.my_port_s = my_port_s
		self.client_host = client_host
		self.client_port = client_port
		self.server_host = server_host
		self.server_port = server_port
		self.end1 = False
		self.end2 = False

		if os.name == "nt":
			s = socket.socket(socket.AF_INET,socket.SOCK_RAW,socket.IPPROTO_IP)
			s.bind(("YOUR_INTERFACE_IP",0))
			s.setsockopt(socket.IPPROTO_IP,socket.IP_HDRINCL,1)
			s.ioctl(socket.SIO_RCVALL,socket.RCVALL_ON)
		else:
			s=socket.socket(socket.PF_PACKET, socket.SOCK_RAW, socket.ntohs(0x0800))

		raw_s = socket.socket(socket.AF_INET, socket.SOCK_RAW, socket.IPPROTO_RAW)

		while self.end1 == False or self.end2 == False:		# When both client and server end connection, finish
			try:
				pkt=s.recvfrom(65565)
				if self.is_my_ip(pkt[0][14:34], True):
					# Cliente?
					if self.is_ip(pkt[0][14:34], True) and self.is_port(pkt[0][34:54], True):
						# Enviar a servidor
						pkt2 = self.to_server_ip(pkt[0][14:34]) + self.to_server_port(pkt[0][34:54], pkt[0][54:]) + pkt[0][54:]
						
						# For testing purposes
						"""self.check_change(pkt[0][14:])
						print("\n---------------------------------------------------------------------->\n")
						self.check_change(pkt2)"""
						
						raw_s.sendto(pkt2, (self.server_host, 0))

				if self.is_my_ip(pkt[0][14:34]):
					if self.is_ip(pkt[0][14:34]) and self.is_port(pkt[0][34:54]):
						# Enviar a cliente
						pkt2 = self.to_client_ip(pkt[0][14:34]) + self.to_client_port(pkt[0][34:54], pkt[0][54:]) + pkt[0][54:]
						
						# For testing purposes
						"""self.check_change(pkt[0][14:])
						print("\n---------------------------------------------------------------------->\n")
						self.check_change(pkt2)"""

						raw_s.sendto(pkt2, (self.client_host, 0))

			except Exception as e:
				print(e)
				break
		
		if self.end1 and self.end2:
			logging.info("Connection from {}:{} ended".format(self.client_host, self.client_port))
			print("Connection from {}:{} ended".format(self.client_host, self.client_port))

def is_new_connection(data, public_port):
	storeobj = struct.unpack('!HHLLBBHHH',data)
	_destination_port = storeobj[1]
	_tcp_flag  =storeobj[5]
	client_port = None

	if _destination_port == public_port:
		client_port = storeobj[0]		# store the client port contained in the packet
	return _destination_port == public_port  and _tcp_flag & 2 != 0, client_port		# SYN flag

def is_my_public_ip(data, public_ip):
		storeobj = struct.unpack("!BBHHHBBH4s4s", data)
		client_address = socket.inet_ntoa(storeobj[8])
		_destination_address = socket.inet_ntoa(storeobj[9])
		return _destination_address == public_ip, client_address

# Tomado de https://www.codeproject.com/Tips/460867/Python-Implementation-of-IP-Checksum
def checksum(ip_header, size):
    
    cksum = 0
    pointer = 0
    
    #The main loop adds up each set of 2 bytes. They are first converted to strings and then concatenated
    #together, converted to integers, and then added to the sum.
    while size > 1:
        cksum += int((str("%02x" % (ip_header[pointer],)) + 
                      str("%02x" % (ip_header[pointer+1],))), 16)
        size -= 2
        pointer += 2
    if size: #This accounts for a situation where the header is odd
        cksum += ip_header[pointer]
        
    cksum = (cksum >> 16) + (cksum & 0xffff)
    cksum += (cksum >>16)
    
    return (~cksum) & 0xFFFF

def to_server_ip_change(data, private_ip, server_host):
		storeobj = struct.unpack("!BBHHHBBH4s4s", data)
		_source_address = socket.inet_aton(private_ip)
		_destination_address = socket.inet_aton(server_host)
		_header_checksum = struct.pack("!BBHHHBBH4s4s", storeobj[0], storeobj[1], storeobj[2], storeobj[3], storeobj[4], storeobj[5], storeobj[6], 0, _source_address, _destination_address)
		_header = struct.pack("!BBHHHBBH4s4s", storeobj[0], storeobj[1], storeobj[2], storeobj[3], storeobj[4], storeobj[5], storeobj[6], checksum(bytearray(_header_checksum), len(_header_checksum)), _source_address, _destination_address)
		return _header

def to_server_port_change(data, private_ip, private_port, server_host, server_port, payload):
	storeobj = struct.unpack('!HHLLBBHHH',data)
	_source_port = private_port
	_destination_port = server_port
	raw = struct.pack("!HHLLBBHHH", _source_port, _destination_port, storeobj[2], storeobj[3], storeobj[4], storeobj[5], storeobj[6], 0, storeobj[8])

	# Get data for checksum
	_source_address = socket.inet_aton(private_ip)
	_destination_address = socket.inet_aton(server_host)
	placeholder = 0
	protocol = socket.IPPROTO_TCP
	tcp_len = len(raw) + len(payload)
	# Assemble pseudo header
	psh = struct.pack('!4s4sBBH', _source_address, _destination_address, placeholder, protocol, tcp_len)
	psh = psh + raw + payload
	# Create header with new checksum
	_header = struct.pack("!HHLLBBHHH", _source_port, _destination_port, storeobj[2], storeobj[3], storeobj[4], storeobj[5], storeobj[6], checksum(bytearray(psh), len(psh)), storeobj[8])
	return _header

# For testing purposes
if __name__ == '__main__':
	if len(sys.argv) != 8:
		print ('Usage:\n\tpython bridge.py <public IP> <private IP> <client port> <server port> <client host> <remote host> <remote port>')
		print ('Example:\n\tpython bridge.py localhost localhost 8080 4444 172.16.202.131 www.google.com 80')
		sys.exit(0)		
	
	public_host = sys.argv[1]
	private_host = sys.argv[2]
	local_port_client = int(sys.argv[3])
	to_server_port = int(sys.argv[4])
	client_host = sys.argv[5]
	server_host = sys.argv[6]
	server_port = int(sys.argv[7])

	bridge = Eth_bridge()
	bridge.run(public_host, private_host, local_port_client, to_server_port, client_host, None, server_host, server_port)
