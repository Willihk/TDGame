using System;
using System.Linq;
using Mirror;
using TDGame.Network.Player;
using UnityEngine;

namespace TDGame.Systems.Economy
{
    public class PlayerEconomyManager : MonoBehaviour
    {
        public static PlayerEconomyManager Instance;
        
        [SerializeField]
        private InGamePlayerManager playerManager;

        private NetworkedPlayerEconomy[] economies;

        private void Awake()
        {
            Instance = this;
        }

        [Server]
        public void AddCurrencyToAllPlayers(int amount)
        {
            GetEconomies();
            Debug.Log(economies.Length);
            foreach (var economy in economies)
            {
                economy.AddCurrency(amount);
            }
        }

        void GetEconomies()
        {
            economies = playerManager.GetPlayerObjects().Select(x => x.GetComponent<NetworkedPlayerEconomy>())
                .ToArray();
        }
    }
}