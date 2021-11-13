using Mirror;
using TDGame.Systems.Health.Unit;
using TDGame.Systems.Old_Enemy.DamageReceiver.Base;
using UnityEngine;

namespace TDGame.Systems.Old_Enemy.DamageReceiver.Implementations
{
    public class BasicDamageReceiver : BaseDamageReceiver
    {
        [SerializeField]
        private NetworkedHealthSystem healthSystem;
        
        [ServerCallback]
        public override void Damage(float damage)
        {
            healthSystem.Damage(damage);
        }
    }
}