using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  SyncFrame  {

    public abstract class InputDataBase
    {
        public byte ownerID;

        public InputDataBase()
        {
        }

        public abstract void Serialize(List<byte> bytes);

        public abstract void Deserialize(byte[] data, ref int offset);

        public abstract bool EqualsData(InputDataBase otherBase);

        public abstract void CleanUp();

        public abstract void CopyFrom(InputDataBase fromBase);
    }
}
