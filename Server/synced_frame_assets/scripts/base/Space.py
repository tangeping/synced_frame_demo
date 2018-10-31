# -*- coding: utf-8 -*-
import KBEngine
import random
import math
from KBEDebug import *

class Space(KBEngine.Entity):
	"""
	一个可操控cellapp上真正space的实体
	注意：它是一个实体，并不是真正的space，真正的space存在于cellapp的内存中，通过这个实体与之关联并操控space。
	"""
	def __init__(self):
		KBEngine.Entity.__init__(self)
		
		self.spaceStateB = self.cellData["spaceState"]
		self.cellData['spaceKeyC'] = self.spaceKey

		DEBUG_MSG("Space[%i]:self.cellData['spaceKeyC']:%i" % (self.id,self.cellData['spaceKeyC']))

		self.createCellEntityInNewSpace(None)	
		
	def onStateChange(self,state):
		if self.spaceStateB != state:
			self.spaceStateB = state
				
	def loginToSpace(self, avatarEntityCall):
		"""
		defined method.
		某个玩家请求登陆到这个space中
		"""
		avatarEntityCall.createCell(self.cell)
		self.onEnter(avatarEntityCall)
		
	def logoutSpace(self, entityID):
		"""
		defined method.
		某个玩家请求登出这个space
		"""
		self.onLeave(entityID)
		

	def onTimer(self, tid, userArg):
		"""
		KBEngine method.
		引擎回调timer触发
		"""
		DEBUG_MSG("%s::onTimer: %i, tid:%i, arg:%i" % (self.getScriptName(), self.id, tid, userArg))

		
	def onEnter(self, entityCall):
		"""
		defined method.
		进入场景
		"""
		
		
		if self.cell is not None:
			self.cell.onEnter(entityCall)
		
	def onLeave(self, entityID):
		"""
		defined method.
		离开场景
		"""	
		if self.cell is not None:
			self.cell.onLeave(entityID)

	def onLoseCell(self):
		"""
		KBEngine method.
		entity的cell部分实体丢失
		"""
		KBEngine.globalData["Spaces"].onSpaceLoseCell(self.spaceKey)

		
	def onGetCell(self):
		"""
		KBEngine method.
		entity的cell部分实体被创建成功
		"""
		DEBUG_MSG("Space::onGetCell: %i" % self.id)
		KBEngine.globalData["Spaces"].onSpaceGetCell(self, self.spaceKey)

		

