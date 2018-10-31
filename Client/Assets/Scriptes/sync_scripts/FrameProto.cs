using KBEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SyncFrame;
using TrueSync;

namespace SyncFrame
{
    public enum CMD
    {
        MOUSE = 1,
        KEYBOARD=2,
        USER = 3,
        TEST = 4,
        BOX  = 5,
        MAX =255,
    }

    public abstract class FrameBase
    {
        public ENTITY_DATA e = new ENTITY_DATA();
        public FixedPointStream s = new FixedPointStream();

        public abstract ENTITY_DATA Serialize();

        public abstract void PareseFrom(ENTITY_DATA e);
    }


    public class FrameMouse : FrameBase
    {
        public TSVector point = TSVector.zero;
        public FrameMouse(CMD cmd, TSVector p)
        {
            e.cmd_type = (Byte)cmd;
            point = p;
        }

        public FrameMouse()
        {
        }

        public override ENTITY_DATA Serialize()
        {
            s.writeTSVector(point);
            e.datas = new byte[s.wpos];
            Array.Copy(s.data(),e.datas, s.wpos);
            return e;
        }

        public override void PareseFrom(ENTITY_DATA e)
        {
            this.e = e;
            s.setBuffer(e.datas);
            point = s.readTSVector();
        }

    }
   
    public class FrameKeyboard : FrameBase
    {
        public List<KeyCode> keys = new List<KeyCode>();

        public FrameKeyboard(CMD cmd, List<KeyCode> keys )
        {
            e.cmd_type = (Byte)cmd;
            this.keys = keys;
        }

        public FrameKeyboard()
        {
        }

        public override ENTITY_DATA Serialize()
        {
            for (int i = 0; i < keys.Count; i++)
            {
                s.writeUint16((UInt16)keys[i]);
            }
            e.datas = new byte[s.wpos];
            Array.Copy(s.data(), e.datas, s.wpos);
            return e;
        }

        public override void PareseFrom(ENTITY_DATA e)
        {
            this.e = e;
            s.setBuffer(e.datas);

            KeyCode k = KeyCode.None;

            while ((k = (KeyCode)s.readUint16()) != 0)
            {
                keys.Add(k);
            }
        }
    }

    public class FrameTest:FrameBase
    {
        public FP fp_1;
        public FP fp_2;


        public FrameTest(CMD cmd, FP arg1,FP arg2)
        {
            e.cmd_type = (Byte)cmd;
            fp_1 = arg1;
            fp_2 = arg2;
        }

        public FrameTest()
        {
        }

        public override ENTITY_DATA Serialize()
        {
            s.writeFP(fp_1);
            s.writeFP(fp_2);
            e.datas = new byte[s.wpos];
            Array.Copy(s.data(), e.datas, s.wpos);
            return e;
        }

        public override void PareseFrom(ENTITY_DATA e)
        {
            this.e = e;
            s.setBuffer(e.datas);
            fp_1 = s.readFP();
            fp_2 = s.readFP();
        }
    }

    public class FrameUser : FrameBase
    {
        public TSVector movement = TSVector.zero;


        public FrameUser(CMD cmd, TSVector p)
        {
            e.cmd_type = (Byte)cmd;
            movement = p;
        }

        public FrameUser()
        {
        }

        public override ENTITY_DATA Serialize()
        {
            s.writeTSVector(movement);
            e.datas = new byte[s.wpos];
            Array.Copy(s.data(), e.datas, s.wpos);
            return e;
        }

        public override void PareseFrom(ENTITY_DATA e)
        {
            this.e = e;
            s.setBuffer(e.datas);
            movement = s.readTSVector();
        }
    }

    public class FrameBox : FrameBase
    {
        public TSVector movement = TSVector.zero;
        public bool space;


        public FrameBox(CMD cmd, TSVector p,bool sp)
        {
            e.cmd_type = (Byte)cmd;
            movement = p;
            space = sp;
        }

        public FrameBox()
        {
        }

        public override ENTITY_DATA Serialize()
        {
            s.writeTSVector(movement);
            s.writeUint8((UINT8)(space ? 1:0));
            e.datas = new byte[s.wpos];
            Array.Copy(s.data(), e.datas, s.wpos);
            return e;
        }

        public override void PareseFrom(ENTITY_DATA e)
        {
            this.e = e;
            s.setBuffer(e.datas);
            movement = s.readTSVector();
            space = s.readUint8() != 0 ;
        }
    }
    public class FrameProto
    {
        static public ENTITY_DATA encode(FrameBase sendMsg)
        {
            return sendMsg.Serialize();
        }
        
        static public FrameBase decode(ENTITY_DATA readMsg)
        {
            FrameBase f;

            switch ((CMD)readMsg.cmd_type)
            {
                case CMD.MOUSE:
                    {
                        f = new FrameMouse();
                    }
                    break;
                case CMD.KEYBOARD:
                    {
                        f = new FrameKeyboard();
                    }
                    break;
                case CMD.USER:
                    {
                        f = new FrameUser();
                    }
                    break;
                case CMD.TEST:
                    {
                        f = new FrameTest();
                    }
                    break;
                case CMD.BOX:
                    {
                        f = new FrameBox();
                    }
                    break;
                default:
                    {
                        f = null;
                    }
                    return f;
            }

            f.PareseFrom(readMsg);

            return f;
        }

    }
}

