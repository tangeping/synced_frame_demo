# -*- coding: utf-8 -*-
import KBEngine
from KBEDebug import *
import GlobalDefine
import d_spaces
from interfaces.GameObject import GameObject
import SCDefine
import copy
import random
import math
import time


from ENTITY_DATA import TEntityFrame
from FRAME_DATA import TFrameData
from FRAME_LIST import TFrameList

class Space(KBEngine.Entity,GameObject):
	"""
	游戏场景，在这里代表野外大地图
	"""
	def __init__(self):
		KBEngine.Entity.__init__(self)
		GameObject.__init__(self)
		
			
		DEBUG_MSG('created space[%d] entityID = %i. spaceKeyC:%i' % (self.spaceID, self.id,self.spaceKeyC))
		
		KBEngine.globalData["space_%i" % self.spaceID] = self.base

		self.avatars = {}

		self.spaceStateChange(GlobalDefine.SPACE_STATE_FREE)

		self.initFrame()
	
	def initFrame(self):
		'''
		'''
		self.spaceFarmeId = 1
		operation = TEntityFrame().createFromDict({"entityid":0,"cmd_type":0,"datas":b''})
		self.emptyFrame = TFrameData().createFromDict({"frameid":0,"operation":[operation]})
		self.currFrame = copy.deepcopy(self.emptyFrame)
		
		self.addTimer(60*60,0,SCDefine.TIMER_TYPE_DESTROY)

	#--------------------------------------------------------------------------------------------
	#                              Callbacks
	#--------------------------------------------------------------------------------------------
	def onDestroy(self):
		"""
		KBEngine method.
		"""
		del KBEngine.globalData["space_%i" % self.spaceID]
		self.destroySpace()
		
	def onEnter(self, entityCall):
		"""
		defined method.
		进入场景
		"""

		DEBUG_MSG('Space::onEnter space[%d] entityID = %i.' % (self.spaceID, entityCall.id))

		
		entity = KBEngine.entities.get(entityCall.id,None)
		if entity:
			entity.position = (-2.0, 1.0, 16.0) if len(self.avatars) <= 0 else (6, 1.0, 16.0)
			entity.component1.isWathcher = len(self.avatars) > 0
			self.avatars[entityCall.id] = entity
		
	def onLeave(self, entityID):
		"""
		defined method.
		离开场景
		"""

		DEBUG_MSG('Space::onLeave space[%d] entityID = %i.' % (self.spaceID, entityID))
		
		if entityID in self.avatars:

			entityBase = self.avatars[entityID].base
			if entityBase is not None:
				entityBase.destoryCell() 
				self.avatars[entityID].component1.allClients.onLoginOutSpaceResult(entityID,self.spaceKeyC)

			del self.avatars[entityID]

	def onTimer(self, tid, userArg):
		"""
		KBEngine method.
		引擎回调timer触发
		"""
		DEBUG_MSG("%s::onTimer: %i, tid:%i, arg:%i" % (self.getScriptName(), self.id, tid, userArg))
		if userArg == SCDefine.TIMER_TYPE_SPACE_TICK:
			self.onBroadFrameBegine()

		elif userArg == SCDefine.TIMER_TYPE_DESTROY:
			self.spaceStateChange(GlobalDefine.SPACE_STATE_DESTORY)
			self.onDestroyTimer()

	def spaceStateChange(self,state):
		"""
		"""
		if self.spaceState != state:
			self.spaceState = state
			self.base.onStateChange(state)

	def onReady(self,entityCall,ready):
		"""
		玩家开始准备
		"""
		if self.spaceState !=  GlobalDefine.SPACE_STATE_FREE:
			return

		entityCall.component1.stateChange(ready)

		self.onCheckGameBegine()

	def onCheckGameBegine(self):

		if len(self.avatars) < 1:
			return False

		for entityCall in self.avatars.values():
			if entityCall.component1.state != GlobalDefine.ENTITY_STATE_READY:
				return False

		self.GameBegine()

	def GameBegine(self):
		
		self.onGameRunning()
		self.addTimer(1,0.00001,SCDefine.TIMER_TYPE_SPACE_TICK)

		for e in self.avatars.values():
			if e is None or e.client is None:
				continue
			randSeed =  int(time.time() * math.pow(10,7))
			e.client.onGameBegine(randSeed)

	def broadMessage(self):

		for e in self.avatars.values():
			if e is None or e.client is None:
				continue
			for frameid in range(e.frameId,self.spaceFarmeId):
				e.client.onRspFrameMessage(self.framePool[frameid+1])
				
			e.frameId = self.spaceFarmeId

	def addFrame(self,entityCall, framedata):
		"""
		添加数据帧
		"""		
		if entityCall is None or self.spaceState != GlobalDefine.SPACE_STATE_PLAYING:
			return

		operation = TEntityFrame().createFromDict({"entityid":framedata[0],"cmd_type":framedata[1],"datas":framedata[2]})

		if self.currFrame[0] <= 0:			
			self.currFrame[1] = [operation]
		else:
			self.currFrame[1].append(operation)		

	def onBroadFrameBegine(self):

		if self.spaceState != GlobalDefine.SPACE_STATE_PLAYING:
			return

		self.currFrame[0] = self.spaceFarmeId
		self.framePool[self.spaceFarmeId] = self.currFrame
		self.broadMessage()

		DEBUG_MSG("Room::onBroadFrameBegine,currFrame:%s" % str(self.currFrame))

		self.onBroadFrameEnd()


	def onBroadFrameEnd(self):
		"""
		广播数据帧后
		"""		

		self.spaceFarmeId += 1
		self.currFrame = copy.deepcopy(self.emptyFrame)


	def onGamePause(self):
		"""
		游戏暂停
		"""
		if self.spaceState != GlobalDefine.SPACE_STATE_PLAYING:
			return
		self.spaceStateChange(GlobalDefine.SPACE_STATE_PAUSE)

	def onGameRunning(self):
		"""
		游戏继续
		"""
		if self.spaceState == GlobalDefine.SPACE_STATE_FREE or self.spaceState == GlobalDefine.SPACE_STATE_PAUSE:
			self.spaceStateChange(GlobalDefine.SPACE_STATE_PLAYING)