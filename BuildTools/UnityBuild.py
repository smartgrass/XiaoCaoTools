#!/usr/bin/python
# -*- coding: utf-8 -*-
import os,sys,string,datetime,time,threading

logFilePath = 'editor.txt'
g_bStop = False

def LoopLog():
	m_logFilePath = logFilePath
	global g_bStop
	nPosRead = 0
	fp = None
	print('OutputLogThread Start')
	while(g_bStop == False):
		if os.path.isfile(m_logFilePath):
			if(fp==None):
				fp = open(m_logFilePath, 'r')

		if fp != None:
			fp.seek(nPosRead)
			allLines = fp.readlines()
			nPosRead = fp.tell()
			fp.close()
			fp = None
			for lines in allLines:
				print(lines)
		time.sleep(4)


def Run():
	if len(sys.argv) < 2:
		print('not find unity path')
		sys.exit(-1)
	unityRunParm = ''
	for i in range(len(sys.argv)):
		if i > 0:
			unityRunParm += ' ' + sys.argv[i]
	unityRunParm += ' -logfile ' + logFilePath
	if os.path.isfile(logFilePath):
		os.remove(logFilePath)

	t1 = threading.Thread(target=LoopLog,args=())
	t1.start()
	os.system(unityRunParm)
	print('Stop!!!')
	global g_bStop
	g_bStop = True
	t1.join()
	print('=======End=====')

if __name__ == '__main__':
	Run()


