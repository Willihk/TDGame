using System;
using TDGame.Events.Base;
using UnityEngine;

namespace TDGame.Systems.Economy.Data
{
    [CreateAssetMenu(fileName = "EconomyData", menuName = "Data/Economy/EconomyData", order = 0)]
    public class EconomyData : ScriptableObject
    {
        [SerializeField]
        private int startCurrency = 100;
        
        [SerializeField]
        private int currency;
        public int Currency => currency;

        [SerializeField]
        private GameEvent<int> economyChanged;


        public void ResetEconomy()
        {
            currency = startCurrency;
        }
        
        public void SetCurrency(int amount)
        {
            currency = amount;
        }

        public void AddCurrency(int amount)
        {
            currency += amount;
        }

        public void ReduceCurrency(int amount)
        {
            currency = Math.Max(currency -= amount, 0);
        }

        public bool CanAfford(int amount)
        {
            return Currency >= amount;
        }
    }
}