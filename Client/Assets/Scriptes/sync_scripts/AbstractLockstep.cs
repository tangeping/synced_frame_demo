using System.Collections;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;

namespace SyncFrame
{
    public abstract class AbstractLockstep
    {
        private FP deltaTime;

        private TrueSyncConfig config;

        private IPhysicsManagerBase physicsManager;

        public static AbstractLockstep NewInstance(FP deltaTime,TrueSyncConfig config, IPhysicsManagerBase physicsManager)
        {
            return new DefaultLockstep(deltaTime, config, physicsManager);
            
        }

        public AbstractLockstep(FP deltaTime, TrueSyncConfig config, IPhysicsManagerBase physicsManager)
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

