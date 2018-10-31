# -*- coding: utf-8 -*-
import KBEngine
from KBEDebug import *

class Operation(KBEngine.EntityComponent):
	def __init__(self):
		KBEngine.EntityComponent.__init__(self)


	def onAttached(self, owner):
		"""
		"""
		INFO_MSG("Operation::onAttached(): owner=%i" % (owner.id))
		
	def onDetached(self, owner):
		"""
		"""
		INFO_MSG("Operation::onDetached(): owner=%i" % (owner.id))


	def onClientEnabled(self):
		"""
		KBEngine method.
		该entity被正式激活为可使用， 此时entity已经建立了client对应实体， 可以在此创建它的
		cell部分。
		"""
		INFO_MSG("Operation[%i]::onClientEnabled:entities enable." % (self.ownerID))

	def onClientDeath(self):
		"""
		KBEngine method.
		客户端对应实体已经销毁
		"""
		DEBUG_MSG("Operation[%i].onClientDeath:" % self.ownerID)


	def onTimer(self, tid, userArg):
		"""
		KBEngine method.
		引擎回调timer触发
		"""
		DEBUG_MSG("%s::onTimer: %i, tid:%i, arg:%i" % (self.name, self.ownerID, tid, userArg))


	def reqSpaceList(self):
		"""
		exposed.
		客户端请求查询房间列表
		"""
		DEBUG_MSG("Operation[%i].reqSpaceList: ." % (self.ownerID))

		KBEngine.globalData["Spaces"].onSpaceList(self.owner)

				
	def reqCreateSpace(self):
		"""
		exposed.
		客户端请求创建一个房间
		"""
		DEBUG_MSG("Operation[%i].reqCreateSpace: ." % (self.ownerID))

		KBEngine.globalData["Spaces"].onCreateSpace(self.owner)
				

	def reqRemoveSpace(self, spaceKey):
		"""
		exposed.
		客户端请求删除一个房间
		"""
		DEBUG_MSG("Operation[%i].reqRemoveSpace: ." % (self.ownerID))

		KBEngine.globalData["Spaces"].onRemoveSpace(spaceKey)

	
	def reqLoginSpace(self, spaceKey):
		"""
		exposed.
		客户端请求进入一个房间
		"""
		DEBUG_MSG("Operation[%i].reqRemoveSpace: ." % (self.ownerID))

		KBEngine.globalData["Spaces"].loginToSpace(self.owner, spaceKey)
		

	def reqLeaveSpace(self, spaceKey):
		"""
		"""
		DEBUG_MSG("Operation[%i].reqLeaveSpace: spaceKey:%i." % (self.ownerID,spaceKey))

		KBEngine.globalData["Spaces"].logoutSpace(self.owner,spaceKey)
