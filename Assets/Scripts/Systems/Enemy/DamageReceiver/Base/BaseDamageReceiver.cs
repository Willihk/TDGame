using Mirror;
using UnityEngine;

namespace TDGame.Systems.Enemy.DamageReceiver.Base
{
    public abstract class BaseDamageReceiver : NetworkBehaviour
    {
        public abstract void Damage(float damage);
    }
}