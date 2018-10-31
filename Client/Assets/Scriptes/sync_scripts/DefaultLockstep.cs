using System.Collections;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;

namespace SyncFrame
{
    public class DefaultLockstep : AbstractLockstep
    {
        public DefaultLockstep(FP deltaTime, TrueSyncConfig config, IPhysicsManagerBase physicsManager) : base(deltaTime, config, physicsManager)
        {
        }
    }
}

