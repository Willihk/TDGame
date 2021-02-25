using System;
using System.Linq;
using Mirror;
using TDGame.Network.Player;
using TDGame.Systems.Economy.Data;
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
            UpdateEconomies();
            foreach (var economy in economies)
            {
                economy.AddCurrency(amount);
            }
        }

        [Server]
        public void ReducesCurrencyForPlayer(int playerId, int amount)
        {
            var economy = playerManager.GetPlayerById(playerId).GetComponent<NetworkedPlayerEconomy>();
            economy.ReduceCurrency(amount);
        }

        public void UpdateEconomies()
        {
            economies = playerManager.GetPlayerObjects().Select(x => x.GetComponent<NetworkedPlayerEconomy>())
                .ToArray();
        }

        public NetworkedPlayerEconomy GetEconomy(int playerId)
        {
            return playerManager.GetPlayerById(playerId).GetComponent<NetworkedPlayerEconomy>();
        }

        public NetworkedPlayerEconomy GetEconomy(NetworkConnection connection)
        {
            return GetEconomy(playerManager.GetIdByConnection(connection));
        }
    }
}