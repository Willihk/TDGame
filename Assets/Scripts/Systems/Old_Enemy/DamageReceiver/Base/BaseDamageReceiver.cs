using Mirror;

namespace TDGame.Systems.Old_Enemy.DamageReceiver.Base
{
    public abstract class BaseDamageReceiver : NetworkBehaviour
    {
        public abstract void Damage(float damage);
    }
}