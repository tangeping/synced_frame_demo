using System.Collections;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;

namespace SyncFrame
{
    public class RollbackLockstep : AbstractLockstep
    {
        public RollbackLockstep(FP deltaTime, TrueSyncConfig config, IPhysicsManagerBase physicsManager) : base(deltaTime, config, physicsManager)
        {
        }
    }

}

