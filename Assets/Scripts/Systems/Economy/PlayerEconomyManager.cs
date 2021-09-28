using System;
using System.Collections.Generic;
using System.IO;
using MessagePack;
using Mirror;
using TDGame.Network.Components;
using TDGame.Network.Components.Messaging;
using TDGame.Network.Player;
using TDGame.Systems.Economy.Messages;
using UnityEngine;
using NetworkConnection = TDGame.Network.Components.Messaging.NetworkConnection;

namespace TDGame.Systems.Economy
{
    public class PlayerEconomyManager : MonoBehaviour
    {
        [SerializeField]
        private PlayerList playerList;

        private Dictionary<int, Economy> economies = new Dictionary<int, Economy>();

        [SerializeField]
        private NetworkPlayerManager playerManager;

        private BaseMessagingManager messagingManager;

        private void Awake()
        {
            messagingManager = BaseMessagingManager.Instance;
            messagingManager.RegisterNamedMessageHandler<SetEconomyMessage>(Handle_SetEconomy);
        }

        private void Start()
        {
            playerManager = NetworkPlayerManager.Instance;

            foreach (var player in playerList.players)
            {
                CreateEconomy(player);
            }
        }

        void CreateEconomy(int playerId)
        {
            var gameObject = new GameObject("Economy - " + playerId);
            gameObject.transform.SetParent(transform);

            var economy = gameObject.AddComponent<Economy>();
            economy.ownerId = playerId;
            economies.Add(playerId, economy);
        }

        public Economy GetEconomy(int playerId)
        {
            return economies[playerId];
        }

        public void AddCurrencyToAllPlayers(int amount)
        {
            foreach (var economy in economies.Values)
            {
                AddCurrencyToEconomy(economy, amount);
            }
        }

        public void ReducesCurrencyForPlayer(int playerId, int amount)
        {
            var economy = GetEconomy(playerId);
            economy.ReduceCurrency(amount);
        }

        public void AddCurrencyToEconomy(int playerId, int amount)
        {
            AddCurrencyToEconomy(economies[playerId], amount);
        }

        public void AddCurrencyToEconomy(Economy economy, int amount)
        {
            // TODO: Sync 
            economy.currency += amount;
            messagingManager.SendNamedMessageToAll(new SetEconomyMessage
                { PlayerId = economy.ownerId, Currency = economy.currency });
        }

        public void ReduceCurrencyToEconomy(int playerId, int amount)
        {
            ReduceCurrencyToEconomy(economies[playerId], amount);
        }

        public void ReduceCurrencyToEconomy(Economy economy, int amount)
        {
            // TODO: Sync 
            economy.currency -= amount;

            messagingManager.SendNamedMessageToAll(new SetEconomyMessage
                { PlayerId = economy.ownerId, Currency = economy.currency });
        }

        void Handle_SetEconomy(NetworkConnection sender, Stream stream)
        {
            var message = MessagePackSerializer.Deserialize<SetEconomyMessage>(stream);

            var economy = GetEconomy(message.PlayerId);

            economy.currency = message.Currency;
        }
    }
}