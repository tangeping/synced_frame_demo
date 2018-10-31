# -*- coding: utf-8 -*-
import KBEngine
import random
from KBEDebug import *
from interfaces.GameObject import GameObject

class Avatar(KBEngine.Entity,GameObject):
	def __init__(self):
		KBEngine.Entity.__init__(self)
		GameObject.__init__(self)

		#self.position = (-2+random.random()*10,1.0,16.0)

	def isPlayer(self):
		"""
		virtual method.
		"""
		return True
		
	def startDestroyTimer(self):
		"""
		virtual method.
		
		启动销毁entitytimer
		"""
		pass

	#--------------------------------------------------------------------------------------------
	#                              Callbacks
	#--------------------------------------------------------------------------------------------
	def onTimer(self, tid, userArg):
		"""
		KBEngine method.
		引擎回调timer触发
		"""
		#DEBUG_MSG("%s::onTimer: %i, tid:%i, arg:%i" % (self.getScriptName(), self.id, tid, userArg))
		GameObject.onTimer(self, tid, userArg)
		
	def onGetWitness(self):
		"""
		KBEngine method.
		绑定了一个观察者(客户端)
		"""
		DEBUG_MSG("Avatar::onGetWitness: %i." % self.id)

	def onLoseWitness(self):
		"""
		KBEngine method.
		解绑定了一个观察者(客户端)
		"""
		DEBUG_MSG("Avatar::onLoseWitness: %i." % self.id)
	
	def onDestroy(self):
		"""
		KBEngine method.
		entity销毁
		"""
		DEBUG_MSG("Avatar::onDestroy: %i." % self.id)

		space = self.getCurrSpace()

		if space:
			space.onLeave(self.id)
		
	def reqFrameChange(self,exposed,framedata):
		"""
		上传操作帧
		"""
		if exposed != self.id:
			return

		self.getCurrSpace().addFrame(self,framedata)