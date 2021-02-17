using System;
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
        public float Health => health;

        private void Awake()
        {
            health = startHealth;
        }

        [Server]
        public void Damage(float amount)
        {
            health -= amount;

            if (health <= 0)
            {
                // TODO: DIE
            }
        }
    }
}