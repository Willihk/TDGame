using System;
using Mirror;
using TDGame.Systems.Health.Data;
using UnityEngine;

namespace TDGame.Systems.Health.Global
{
    [Obsolete]
    public class GlobalHealthSystem : NetworkBehaviour
    {
        public static GlobalHealthSystem Instance;
    
        [SerializeField]
        private HealthData healthData;

        [SyncVar(hook = nameof(SyncToServer))]
        public float health;

        public override void OnStartServer()
        {
            Instance = this;
            base.OnStartServer();
            healthData.ResetHealth();
            health = healthData.Health;
        }

        [Server]
        public void ReduceHealth(float amount)
        {
            healthData.Reduce(amount);
            health = healthData.Health;
        }

        void SyncToServer(float oldHealth, float newHealth)
        {
            healthData.Set(newHealth);
        }
    }
}