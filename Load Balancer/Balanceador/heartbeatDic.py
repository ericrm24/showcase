CHECK_TIMEOUT = 5

import socket
import threading
import time
import sys
sys.path.insert(0, './Herramientas')

class Heartbeats(dict):

    def __init__(self):
        super(Heartbeats, self).__init__()
        self._lock = threading.Lock()

    def __setitem__(self, key, value):
        #Create or update the dictionary entry for a server
        self._lock.acquire()
        super(Heartbeats, self).__setitem__(key, value)
        self._lock.release()

    def noHearbeat(self):
        #Return a list of servers with heartbeat older than CHECK_TIMEOUT
        limit = time.time() - CHECK_TIMEOUT
        self._lock.acquire()
        silent = [ip for (ip, ipTime) in self.items() if ipTime < limit]

        for ip in silent:	
            self.pop(ip)

        self._lock.release()
        return silent


