using System.Collections;
using System.Collections.Generic;
using KBEngine;
using UnityEngine;

namespace SyncFrame
{
    public abstract class AbstractLockstep
    {
        private FP deltaTime;

        private FrameSyncConfig config;

        private IPhysicsManagerBase physicsManager;

        public static AbstractLockstep NewInstance(FP deltaTime,FrameSyncConfig config, IPhysicsManagerBase physicsManager)
        {
            return new DefaultLockstep(deltaTime, config, physicsManager);
            
        }

        public AbstractLockstep(FP deltaTime, FrameSyncConfig config, IPhysicsManagerBase physicsManager)
        {
            this.deltaTime = deltaTime;
            this.config = config;
            this.physicsManager = physicsManager;
        }

        public void Update()
        {

        }
    }
}

