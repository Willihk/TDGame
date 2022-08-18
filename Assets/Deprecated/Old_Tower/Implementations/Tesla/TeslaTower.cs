using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using TDGame.Systems.Old_Enemy.DamageReceiver.Base;
using TDGame.Systems.Stats;
using TDGame.Systems.Targeting.Implementations;
using TDGame.Systems.Tower.Base;
using UnityEngine;
using UnityEngine.Events;

namespace TDGame.Systems.Tower.Implementations.Tesla
{
    [Obsolete]
    public class TeslaTower : BaseNetworkedTower
    {
        public MultiTargetSystem targetSystem;

        public UnityEvent ClientHitEvent;

        [Header("Stats")]
        [Space(10)]
        [SerializeField]
        protected StatWrapper hitDamageStat;

        [SerializeField]
        protected StatWrapper hitRateStat;

        private float nexthit;

        private void Update()
        {
            if (isServer)
            {
                if (nexthit < Time.time)
                {
                    Hit();
                }
            }
        }

        [Server]
        void Hit()
        {
            nexthit = Time.time + hitRateStat.stat.Value;

            foreach (var damageReceiver in targetSystem.targets.Select(x => x.GetComponent<BaseDamageReceiver>()))
            {
                damageReceiver.Damage(hitDamageStat.stat.Value);
            }

            Rpc_DummyHit();
        }

        [ClientRpc]
        void Rpc_DummyHit()
        {
            ClientHitEvent.Invoke();
        }
    }
}