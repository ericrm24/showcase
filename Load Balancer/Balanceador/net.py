import fcntl
import ipaddress
import socket
import struct

def get_ip_address(interface):
	s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
	return socket.inet_ntoa(fcntl.ioctl(s.fileno(), 0x8915, struct.pack('256s', bytes(interface[:15], 'utf-8')))[20:24])

def get_netmask(interface):
	s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
	return socket.inet_ntoa(fcntl.ioctl(s.fileno(), 0x891b, struct.pack('256s', bytes(interface, 'utf-8'))) [20:24])

def get_broadcast_ip(ip, mask):
	net = ipaddress.IPv4Network(ip + '/' + mask, False)
	return net.broadcast_address