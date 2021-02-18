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

        private void Awake()
        {
            health = startHealth;
        }

        public void ResetHealth()
        {
            health = startHealth;
        }

        public void Set(float value)
        {
            health = value;
            healthChangedEvent.Raise(health);
        }

        public void Reduce(float amount)
        {
            health -= amount;

            if (health <= 0)
            {
                health = 0;
                healthDepletedEvent.Raise();
            }

            healthChangedEvent.Raise(health);
        }

        public void Add(float amount)
        {
            health = Math.Max(health + amount, maxHealth);
            
            healthChangedEvent.Raise(health);
        }
    }
}