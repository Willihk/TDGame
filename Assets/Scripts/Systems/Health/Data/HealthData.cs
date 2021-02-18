using System;
using TDGame.Events.Base;
using UnityEngine;

namespace TDGame.Systems.Health.Data
{
    [CreateAssetMenu(fileName = "HealthData", menuName = "Data/Health/HealthData", order = 0)]
    public class HealthData : ScriptableObject
    {
        [SerializeField]
        private float maxHealth = 100;
        
        [SerializeField]
        private float startHealth = 100;
        
        [NonSerialized]
        private float health;
        
        public bool IsAtMaxHealth => health >= startHealth;
        public float Health => health;
        
        [SerializeField]
        private GameEvent<float> healthChangedEvent;

        [SerializeField]
        private GameEvent healthDepletedEvent;

        public void Reduce(float amount)
        {
            health -= amount;
            healthChangedEvent.Raise(health);

            if (health <= 0)
            {
                healthDepletedEvent.Raise();
            }
        }

        public void Add(float amount)
        {
            health = Math.Max(health + amount, maxHealth);
            
            healthChangedEvent.Raise(health);
        }
    }
}