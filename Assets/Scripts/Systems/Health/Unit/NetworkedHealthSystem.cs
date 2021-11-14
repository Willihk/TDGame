using System;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace TDGame.Systems.Health.Unit
{
    [Obsolete]
    public class NetworkedHealthSystem : NetworkBehaviour
    {
        [SerializeField]
        protected float maxHealth;

        [SerializeField]
        protected float startHealth;

        [SyncVar]
        private float health;

        public UnityEvent OnDeath;
        public UnityEvent ClientOnDeath;
        public UnityEvent OnHealthChanged;

        public bool IsAtMaxHealth => health >= startHealth;
        public float Health => health;

        private void Awake()
        {
            health = startHealth;
            OnHealthChanged ??= new UnityEvent();
            OnDeath ??= new UnityEvent();
            ClientOnDeath ??= new UnityEvent();
        }

        [ClientRpc]
        void Rpc_DeathEvent()
        {
            if (isServer)
                return;

            ClientOnDeath.Invoke();
        }

        [Server]
        public void Damage(float amount)
        {
            health -= amount;
            OnHealthChanged.Invoke();

            if (health <= 0)
            {
                Rpc_DeathEvent();

                // The enemy object is already destroyed before Rpc_DeathEvent() is called, so the event is manually called.
                if (isClient)
                    ClientOnDeath.Invoke();

                OnDeath.Invoke();
            }
        }
    }
}