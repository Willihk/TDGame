using System;
using System.Linq;
using Mirror;
using TDGame.Systems.Old_Enemy.DamageReceiver.Base;
using TDGame.Systems.Stats;
using TDGame.Systems.Targeting.Implementations;
using TDGame.Systems.Tower.Base;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace TDGame.Systems.Tower.Implementations
{
    [Obsolete]
    public class AoETower : BaseNetworkedTower
    {
        public MultiTargetSystem targetSystem;
        
        [SerializeField]
        protected NetworkedStatsController statsController;
        [SerializeField]
        protected string hitDamageStatName = "HitDamage";

        [SerializeField]
        public UnityEvent clientHitEvent;
        [SerializeField]
        public UnityEvent<Vector3> clientTargetHitEvent;
        
        protected StatWrapper hitDamageStat;

        private void Start()
        {
            hitDamageStat = statsController.GetStatByName(hitDamageStatName);
        }

        public void HitTargets()
        {
            if (!isServer)
                return;
            
            DummyHit();
            
            foreach (var damageReceiver in targetSystem.targets.Select(x => x.GetComponent<BaseDamageReceiver>()))
            {
                damageReceiver.Damage(hitDamageStat.stat.Value);
                Rpc_DummyTargetHit(damageReceiver.transform.position);
            }
        }

        [ClientRpc]
        void DummyHit()
        {
            clientHitEvent.Invoke();
        }
        
        [ClientRpc]
        void Rpc_DummyTargetHit(Vector3 position)
        {
            clientTargetHitEvent.Invoke(position);
        }
    }
}