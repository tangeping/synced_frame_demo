using System.Collections;
using System.Collections.Generic;
using System.IO;
using KBEngine;
using KBEngine;

namespace SyncFrame
{
    public class FixedPointStream : KBEngine.MemoryStream
    {

        public FP readFP()
        {
            return FP.FromRaw(readInt64());
        }

        public TSVector2 readTSVector2()
        {
            return new TSVector2(readFP(), readFP());
        }

        public TSVector readTSVector()
        {
            return new TSVector(readFP(), readFP(), readFP());
        }


        public void writeFP(FP v)
        {
            writeInt64(v.RawValue);
        }

        public void writeTSVector2(TSVector2 v)
        {
            writeFP(v.x);
            writeFP(v.y);
        }

        public void writeTSVector(TSVector v)
        {
            writeFP(v.x);
            writeFP(v.y);
            writeFP(v.z);
        }
    }
}
