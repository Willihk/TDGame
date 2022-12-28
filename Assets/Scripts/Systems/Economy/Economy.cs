using Sirenix.OdinInspector;
using UnityEngine;

namespace TDGame.Systems.Economy
{
    public class Economy : MonoBehaviour
    {
        
        // Owner of the economy. only the owner is able to consume the economy
        public int ownerId;
        
        public int currency;

        [Button]
        public void ReduceCurrency(int amount)
        {
            currency -= amount;
        }

        [Button]
        public void AddCurrency(int amount)
        {
            currency += amount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount">The amount to purchase for</param>
        /// <returns>True if the purchase went through</returns>
        public bool Purchase(int amount)
        {
            if (!CanAfford(amount))
                return false;

            ReduceCurrency(amount);
            return true;
        }

        public bool CanAfford(int amount)
        {
            return currency >= amount;
        }
    }
}