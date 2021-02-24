using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using TDGame.Enemy.Base;
using TDGame.Systems.Targeting.Implementations;
using TDGame.Systems.Turrets.Base;
using UnityEngine;
using UnityEngine.Events;

namespace TDGame.Systems.Turrets.Implementations
{
    public class TeslaTurret : BaseNetworkedTurret
    {
        [SerializeField]
        private MultiTargetSystem targetSystem;

        private List<GameObject> targets = new List<GameObject>();

        public UnityEvent ClientHitEvent;

        private SyncList<Vector3> syncedTargetPositions = new SyncList<Vector3>();

        [Header("Stats")]
        [Space(10)]
        [SerializeField]
        protected float hitDamage = 5;

        [SerializeField]
        protected float hitRate = .4f;

        private float nexthit;

        private void Update()
        {
            if (isServer)
            {
                if (IsTargetUpdateNeeded())
                {
                    UpdateTargets();
                    return;
                }

                syncedTargetPositions.Clear();
                syncedTargetPositions.AddRange(targets.Select(x => x.transform.position));

                if (nexthit < Time.time)
                {
                    Hit();
                }
            }
        }

        [Server]
        void Hit()
        {
            nexthit = Time.time + hitRate;

            foreach (var target in targets.Select(x => x.GetComponent<NetworkedEnemy>()))
            {
                target.Damage(hitDamage);
            }

            Rpc_DummyHit();
        }

        [ClientRpc]
        void Rpc_DummyHit()
        {
            ClientHitEvent.Invoke();
        }

        void UpdateTargets()
        {
            targets.Clear();
            targets.AddRange(targetSystem.GetTargets());
        }

        bool IsTargetUpdateNeeded()
        {
            return targets.Any(x => x == null) || targets.Count < targetSystem.maxTargets || targets.Any(x => targetSystem.IsValidTarget(x));
        }
    }
}