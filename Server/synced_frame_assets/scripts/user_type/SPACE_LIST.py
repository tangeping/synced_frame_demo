# -*- coding: utf-8 -*-
import KBEngine
from KBEDebug import *


class TSpaceList(dict):
    """
    """
    def __init__(self):
        """
        """
        dict.__init__(self)

    def asDict(self):

        datas = []

        for key,val in self.items():
            datas.append(val)

        dic = {
            "values": datas,
        }

        return dic

    def createFromDict(self, dictData):
        for data in dictData["values"]:
            self[data[0]] = data
        return self


class SPACE_LIST_PICKLER:
    def __init__(self):
        pass

    def createObjFromDict(self, dict):
        return TSpaceList().createFromDict(dict)

    def getDictFromObj(self, obj):
        return obj.asDict()

    def isSameType(self, obj):
        return isinstance(obj, TSpaceList)


inst = SPACE_LIST_PICKLER()