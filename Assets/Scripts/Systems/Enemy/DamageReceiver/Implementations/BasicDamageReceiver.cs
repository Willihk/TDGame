using Mirror;
using TDGame.Systems.Enemy.DamageReceiver.Base;
using TDGame.Systems.Health.Unit;
using UnityEngine;

namespace TDGame.Systems.Enemy.DamageReceiver.Implementations
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