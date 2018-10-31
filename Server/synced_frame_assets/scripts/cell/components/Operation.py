# -*- coding: utf-8 -*-
import KBEngine
import GlobalDefine
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


	def reqLoginOutSpace(self, exposed):
		"""
		"""
		if exposed != self.ownerID:
			return

		space = self.owner.getCurrSpace()
		if space is None:
			return

		DEBUG_MSG("Operation[%i].reqLoginOutSpace: spaceKeyC:%i" % (self.ownerID,space.spaceKeyC))

		self.base.reqLeaveSpace(space.spaceKeyC)

	def stateChange(self,ready):
		"""
		改变玩家状态
		"""
		if self.state != ready:
			self.state = ready

	def getState(self):
		"""

		"""
		return self.state

	def reqReady(self, exposed,ready):
		"""
		exposed.
		客户端请求删除一个角色
		"""
		if exposed != self.owner.id:
			return
		space = self.owner.getCurrSpace()
		if space:
			space.onReady(self.owner,ready)

		DEBUG_MSG("Operation[%i].reqReady:%d ." % (self.owner.id,ready))


	def reqGamePause(self,exposed):

		if exposed != self.owner.id:
			return
		space = self.owner.getCurrSpace()
		if space:
			space.onGamePause()

	def reqGameRunning(self,exposed):

		if exposed != self.owner.id:
			return
		space = self.owner.getCurrSpace()
		if space:
			space.onGameRunning()
