using System.Linq;
using Mirror;
using TDGame.Network.Player;
using UnityEngine;

namespace TDGame.Systems.Economy.Old
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

        private void UpdateEconomies()
        {
            economies = playerManager.GetPlayerObjects().Select(x => x.GetComponent<NetworkedPlayerEconomy>())
                .ToArray();
        }

        private NetworkedPlayerEconomy GetEconomy(int playerId)
        {
            return playerManager.GetPlayerById(playerId).GetComponent<NetworkedPlayerEconomy>();
        }

        public NetworkedPlayerEconomy GetEconomy(NetworkConnection connection)
        {
            return GetEconomy(playerManager.GetIdByConnection(connection));
        }
    }
}