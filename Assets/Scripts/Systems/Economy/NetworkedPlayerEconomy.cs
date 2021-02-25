using Mirror;
using TDGame.Systems.Economy.Data;
using UnityEngine;

namespace TDGame.Systems.Economy
{
    public class NetworkedPlayerEconomy : NetworkBehaviour
    {
        [SerializeField]
        private EconomyData localEconomy;

        [SerializeField]
        [SyncVar(hook = nameof(UpdateCurrency))]
        private int syncedCurrency;

        public override void OnStartServer()
        {
            base.OnStartServer();
            syncedCurrency = localEconomy.startCurrency;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            UpdateCurrency(0, syncedCurrency);
        }

        [Server]
        public void ReduceCurrency(int amount)
        {
            syncedCurrency -= amount;
        }

        [Server]
        public void AddCurrency(int amount)
        {
            syncedCurrency += amount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount">The amount to purchase for</param>
        /// <returns>True if the purchase went through</returns>
        [Server]
        public bool Purchase(int amount)
        {
            if (!CanAfford(amount))
                return false;

            ReduceCurrency(amount);
            return true;
        }
        
        public bool CanAfford(int amount)
        {
            return syncedCurrency >= amount;
        }

        void UpdateCurrency(int oldCurrency, int newCurrency)
        {
            if (hasAuthority)
                localEconomy.SetCurrency(newCurrency);
        }
    }
}