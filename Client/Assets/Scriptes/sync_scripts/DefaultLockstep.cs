using System.Collections;
using System.Collections.Generic;
using KBEngine;
using UnityEngine;

namespace SyncFrame
{
    public class DefaultLockstep : AbstractLockstep
    {
        public DefaultLockstep(FP deltaTime, FrameSyncConfig config, IPhysicsManagerBase physicsManager) : base(deltaTime, config, physicsManager)
        {
        }
    }
}

