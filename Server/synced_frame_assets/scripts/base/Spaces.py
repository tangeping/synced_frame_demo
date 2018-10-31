# -*- coding: utf-8 -*-
import KBEngine
import Functor
import SCDefine
import Watcher
import d_spaces
import GlobalDefine
from KBEDebug import *

from SPACE_INFO import TSpaceInfo
from SPACE_LIST import TSpaceList



class Spaces(KBEngine.Entity):
	"""
	这是一个脚本层封装的空间管理器
	KBEngine的space是一个抽象空间的概念，一个空间可以被脚本层视为游戏场景、游戏房间、甚至是一个宇宙。
	"""
	def __init__(self):
		KBEngine.Entity.__init__(self)
		
		# 向全局共享数据中注册这个管理器的entityCall以便在所有逻辑进程中可以方便的访问
		KBEngine.globalData["Spaces"] = self
	
		self._spaceAllocs = {}
	
	def getSpaceAllocs(self):
		return self._spaceAllocs
				
	def loginToSpace(self, avatarEntity, spaceKey):
		"""
		defined method.
		某个玩家请求登陆到某个space中
		"""
		result = 0 if spaceKey not in self._spaceAllocs else spaceKey
		avatarEntity.component1.client.onLoginSpaceResult(result)

		if spaceKey in self._spaceAllocs:
			self._spaceAllocs[spaceKey]['player_count'] += 1
			self._spaceAllocs[spaceKey]['spaceEntityCall'].loginToSpace(avatarEntity)
	

	def logoutSpace(self, avatarEntity, spaceKey):
		"""
		defined method.
		某个玩家请求登出这个space
		"""	
		if spaceKey not in self._spaceAllocs:
			avatarEntity.component1.client.onLoginOutSpaceResult(avatarEntity.id,0)

		if spaceKey in self._spaceAllocs:
			self._spaceAllocs[spaceKey]['player_count'] -= 1
			self._spaceAllocs[spaceKey]['spaceEntityCall'].logoutSpace(avatarEntity.id)


	def onSpaceList(self,avatarEntity):
		'''
		define method.
		玩家请求获取空间列表
		'''
		spaceList = TSpaceList()

		for spaceKey , spaceData in self._spaceAllocs.items():
			if spaceData['spaceEntityCall'] is None:
				continue
			spaceInfo = TSpaceInfo().createFromDict({
			"space_key"		:spaceData['space_key'],
			"player_count"	:spaceData['player_count'],
			"space_state"	:spaceData['spaceEntityCall'].spaceStateB,
			"space_creater"	:spaceData['space_creater']})
			spaceList[spaceKey] = spaceInfo

		avatarEntity.component1.client.onReqSpaceList(spaceList)

				
	def onCreateSpace(self,avatarEntity):
		"""
		客户端请求创建一个空间
		"""
		spaceKey = KBEngine.genUUID64()

		KBEngine.createEntityAnywhere('Space', \
											{\
											"spaceKey" : spaceKey,	\
											"spaceState": GlobalDefine.SPACE_STATE_UNKNOW,\
											}, \
											Functor.Functor(self.onSpaceCreatedCB, spaceKey,avatarEntity))
	
	def onSpaceCreatedCB(self, spaceKey, avatarEntity,space):
		"""
		一个space创建好后的回调
		"""
		DEBUG_MSG("Spaces::onSpaceCreatedCB: space %i. entityID=%i" % (spaceKey, space.id))

		result = 0 if space is  None else spaceKey
		avatarEntity.component1.client.onCreateSpaceResult(result)

		if space :
			self._spaceAllocs[spaceKey] = {'space_key':spaceKey,\
											'player_count':0,\
											'space_state': GlobalDefine.SPACE_STATE_UNKNOW,\
											'space_creater':avatarEntity.id,\
											'spaceEntityCall':None}

	def onRemoveSpace(self, spaceKey):
		"""
		exposed.
		客户端请求删除一个空间
		"""
		space = self.spaceAlloc.get(spaceKey)
		if space is None:
			return

		if space.cell is not None:
			self.destroyCellEntity()

		del self.spaceAlloc[spaceKey]
		
	#--------------------------------------------------------------------------------------------
	#                              Callbacks
	#--------------------------------------------------------------------------------------------
	def onTimer(self, tid, userArg):
		"""
		KBEngine method.
		引擎回调timer触发
		"""
		DEBUG_MSG("%s::onTimer: %i, tid:%i, arg:%i" % (self.getScriptName(), self.id, tid, userArg))

		
	def onSpaceLoseCell(self,spaceKey):
		"""
		defined method.
		space的cell创建好了
		"""
		if spaceKey in self._spaceAllocs:
			del self._spaceAllocs[spaceKey]

		
	def onSpaceGetCell(self,spaceEntityCall, spaceKey):
		"""
		defined method.
		space的cell创建好了
		"""
		DEBUG_MSG("Spaces::onSpaceGetCell.space.%s,spaceKey:%d"%(spaceEntityCall.id,spaceKey))
		
		if spaceKey in self._spaceAllocs:
			self._spaceAllocs[spaceKey]['spaceEntityCall'] = spaceEntityCall

