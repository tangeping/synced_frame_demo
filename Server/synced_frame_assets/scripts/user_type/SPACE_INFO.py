# -*- coding: utf-8 -*-
import KBEngine
from KBEDebug import *


class TSpaceInfo(list):
    """
    """
    def __init__(self):
        """
        """
        list.__init__(self)

    def asDict(self):
        data = {
            "space_key"         : self[0],
            "player_count"      : self[1],
            "space_state"       : self[2],
            "space_creater"     : self[3],
        }
        return data

    def createFromDict(self, dictData):
        self.extend([dictData["space_key"], dictData["player_count"], dictData["space_state"], dictData["space_creater"]])
        return self


class SPACE_INFO_PICKLER:
    def __init__(self):
        pass

    def createObjFromDict(self, dict):
        return TSpaceInfo().createFromDict(dict)

    def getDictFromObj(self, obj):
        return obj.asDict()

    def isSameType(self, obj):
        return isinstance(obj, TSpaceInfo)


inst = SPACE_INFO_PICKLER()


