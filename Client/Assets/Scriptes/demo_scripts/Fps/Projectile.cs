using System;
using System.Collections;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public KBEngine.Avatar owner;
    public FP speed = 15;
    public TSVector direction;

    private FP destroyTime = 3;

    public void OnSyncedUpdate()
    {
//         if (destroyTime <= 0)
//         {
//             TrueSyncManager.SyncedDestroy(this.gameObject);
//         }
//         tsTransform.Translate(direction * speed * TrueSyncManager.DeltaTime);
//         destroyTime -= TrueSyncManager.DeltaTime;
    }

    public void OnSyncedTriggerEnter(TSCollision other)
    {
//         if (other.gameObject.tag == "Player")
//         {
//             PlayerMovement hitPlayer = other.gameObject.GetComponent<PlayerMovement>();
//             if (hitPlayer.owner != owner)
//             {
//                 TrueSyncManager.SyncedDestroy(this.gameObject);
//                 hitPlayer.Respawn();
//             }
//         }
    }
}