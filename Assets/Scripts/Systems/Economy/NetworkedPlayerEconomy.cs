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
            localEconomy.ResetEconomy();
        }

        [Server]
        public void ReduceCurrency(int amount)
        {
            localEconomy.ReduceCurrency(amount);
        }

        [Server]
        public void AddCurrency(int amount)
        {
            localEconomy.AddCurrency(amount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount">The amount to purchase for</param>
        /// <returns>True if the purchase went through</returns>
        [Server]
        public bool Purchase(int amount)
        {
            if (!localEconomy.CanAfford(amount))
                return false;

            ReduceCurrency(amount);
            return true;
        }

        void UpdateCurrency(int oldCurrency, int newCurrency)
        {
            if (hasAuthority)
                localEconomy.SetCurrency(newCurrency);
        }
    }
}