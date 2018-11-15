using System.Collections;
using System.Collections.Generic;
using KBEngine;
using UnityEngine;

namespace SyncFrame
{
    public class RollbackLockstep : AbstractLockstep
    {
        public RollbackLockstep(FP deltaTime, FrameSyncConfig config, IPhysicsManagerBase physicsManager) : base(deltaTime, config, physicsManager)
        {
        }
    }

}

