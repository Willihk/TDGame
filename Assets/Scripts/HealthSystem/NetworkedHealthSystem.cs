using Mirror;
using UnityEngine;

namespace TDGame.HealthSystem
{
    public class NetworkedHealthSystem : NetworkBehaviour
    {
        [SerializeField]
        protected float maxHealth;
        
        [SerializeField]
        protected float startHealth;
        
        [SyncVar]
        protected float health;
        
        public bool IsAtMaxHealth => health >= startHealth;

        [Command]
        public void Damage(float amount)
        {
            health -= health;

            if (health <= 0)
            {
                // TODO: DIE
            }
        }
    }
}