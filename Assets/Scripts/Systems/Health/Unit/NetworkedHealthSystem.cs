using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace TDGame.Systems.Health.Unit
{
    public class NetworkedHealthSystem : NetworkBehaviour
    {
        [SerializeField]
        protected float maxHealth;
        
        [SerializeField]
        protected float startHealth;
        
        [SyncVar]
        private float health;

        public UnityEvent OnDeath;
        public UnityEvent OnHealthChanged;
        
        public bool IsAtMaxHealth => health >= startHealth;
        public float Health => health;

        private void Awake()
        {
            health = startHealth;
            OnHealthChanged ??= new UnityEvent();
            OnDeath ??= new UnityEvent();
        }

        [Server]
        public void Damage(float amount)
        {
            health -= amount;
            OnHealthChanged.Invoke();

            if (health <= 0)
            {
                // TODO: DIE
                OnDeath.Invoke();
            }
        }
    }
}