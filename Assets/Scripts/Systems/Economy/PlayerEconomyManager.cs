using System.Collections.Generic;
using System.IO;
using System.Linq;
using MessagePack;
using Sirenix.OdinInspector;
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
            messagingManager.RegisterNamedMessageHandler<SetEconomiesMessage>(Handle_SetEconomies);

            messagingManager.RegisterNamedMessageHandler<RequestEconomiesMessage>(Handle_RequestEconomies);
        }

        private void Start()
        {
            playerManager = NetworkPlayerManager.Instance;

            if (CustomNetworkManager.Instance.serverWrapper.isListening)
            {
                foreach (var player in playerList.players)
                {
                    CreateEconomy(player);
                }
            }


            if (CustomNetworkManager.Instance.clientWrapper.isConnected)
            {
                messagingManager.SendNamedMessageToServer(new RequestEconomiesMessage());
            }
        }

        Economy CreateEconomy(int playerId)
        {
            var gameObject = new GameObject("Economy - " + playerId);
            gameObject.transform.SetParent(transform);

            var economy = gameObject.AddComponent<Economy>();
            economy.ownerId = playerId;
            economies.Add(playerId, economy);
            return economy;
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

        [Button]
        public void AddCurrencyToEconomy(int playerId, int amount)
        {
            AddCurrencyToEconomy(economies[playerId], amount);
        }

        public void AddCurrencyToEconomy(Economy economy, int amount)
        {
            if (!CustomNetworkManager.Instance.serverWrapper.isListening) // Only run on the server
                return;

            // TODO: Sync 
            economy.currency += amount;
            messagingManager.SendNamedMessageToAll(new SetEconomyMessage
                { PlayerId = economy.ownerId, Currency = economy.currency });
        }

        [Button]
        public void ReduceCurrencyToEconomy(int playerId, int amount)
        {
            ReduceCurrencyToEconomy(economies[playerId], amount);
        }

        public void ReduceCurrencyToEconomy(Economy economy, int amount)
        {
            if (!CustomNetworkManager.Instance.serverWrapper.isListening) // Only run on the server
                return;

            // TODO: Sync 
            economy.currency -= amount;

            messagingManager.SendNamedMessageToAll(new SetEconomyMessage
                { PlayerId = economy.ownerId, Currency = economy.currency });
        }

        void Handle_RequestEconomies(NetworkConnection sender, Stream stream)
        {
            var messages = economies.Values.Select(x => new SetEconomyMessage
                { Currency = x.currency, PlayerId = x.ownerId }).ToArray();

            messagingManager.SendNamedMessage(sender, new SetEconomiesMessage { EconomyMessages = messages });
        }

        void Handle_SetEconomies(NetworkConnection sender, Stream stream)
        {
            var message = MessagePackSerializer.Deserialize<SetEconomiesMessage>(stream);

            for (int i = 0; i < economies.Count; i++)
            {
                Destroy(economies.Values.ToArray()[i].gameObject);
            }

            economies.Clear();


            foreach (var item in message.EconomyMessages)
            {
                var economy = CreateEconomy(item.PlayerId);
                economy.currency = item.Currency;
            }
        }

        void Handle_SetEconomy(NetworkConnection sender, Stream stream)
        {
            var message = MessagePackSerializer.Deserialize<SetEconomyMessage>(stream);

            var economy = GetEconomy(message.PlayerId);

            economy.currency = message.Currency;
        }
    }
}