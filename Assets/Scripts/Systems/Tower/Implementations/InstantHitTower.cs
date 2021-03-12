using Mirror;
using TDGame.Systems.Enemy.DamageReceiver.Base;
using TDGame.Systems.Stats;
using TDGame.Systems.Targeting.Implementations;
using TDGame.Systems.Tower.Base;
using UnityEngine;
using UnityEngine.Events;

namespace TDGame.Systems.Tower.Implementations
{
    public class InstantHitTower : BaseNetworkedTower
    {
        [SerializeField]
        protected SingleTargetSystem targetSystem;

        public UnityEvent ClientHitEvent;

        [Header("Stats")]
        [Space(10)]
        [SerializeField]
        protected StatWrapper hitDamage;

        [SerializeField]
        protected StatWrapper hitRate;

        private float nextHit;

        void Update()
        {
            if (isServer)
            {
                if (nextHit < Time.time)
                {
                    Hit();
                }
            }
        }

        private void Hit()
        {
            nextHit = Time.time + hitRate.stat.Value;

            targetSystem.target.GetComponent<BaseDamageReceiver>().Damage(hitDamage.stat.Value);

            Rpc_DummyHit();
        }

        [ClientRpc]
        void Rpc_DummyHit()
        {
            ClientHitEvent.Invoke();
        }
    }
}
