using System.Collections.Generic;
using System.IO;
using System.Linq;
using MessagePack;
using Sirenix.OdinInspector;
using TDGame.Events;
using TDGame.Network.Components;
using TDGame.Network.Components.Messaging;
using TDGame.Network.Player;
using TDGame.Systems.Economy.Messages;
using UnityEngine;

namespace TDGame.Systems.Economy
{
    public class PlayerEconomyManager : MonoBehaviour
    {
        public static PlayerEconomyManager Instance;

        [SerializeField]
        private PlayerList playerList;

        private Dictionary<int, Economy> economies = new();

        [SerializeField]
        private NetworkPlayerManager playerManager;

        private BaseMessagingManager messagingManager;

        private EventManager eventManager;


        private void Awake()
        {
            Instance = this;

            messagingManager = BaseMessagingManager.Instance;
            messagingManager.RegisterNamedMessageHandler<SetEconomyMessage>(Handle_SetEconomy);
            messagingManager.RegisterNamedMessageHandler<SetEconomiesMessage>(Handle_SetEconomies);

            messagingManager.RegisterNamedMessageHandler<RequestEconomiesMessage>(Handle_RequestEconomies);
        }

        private void Start()
        {
            playerManager = NetworkPlayerManager.Instance;
            eventManager = EventManager.Instance;

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
            AddCurrencyToEconomy(economy, 10);
            economies.Add(playerId, economy);
            return economy;
        }

        public Economy GetEconomy(int playerId)
        {
            return economies[playerId];
        }

        public void AddCurrencyToAllPlayers(int amount)
        {
            if (!CustomNetworkManager.Instance.serverWrapper.isListening) // Only run on the server
                return;
            
            
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

        public void SyncEconomies()
        {
            foreach ((int id, var economy) in economies)
            {
                messagingManager.SendNamedMessageToAll(new SetEconomyMessage
                    { PlayerId = id, Currency = economy.currency });
            }
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

        void Handle_RequestEconomies(TDNetworkConnection sender, Stream stream)
        {
            var messages = economies.Values.Select(x => new SetEconomyMessage
                { Currency = x.currency, PlayerId = x.ownerId }).ToArray();

            messagingManager.SendNamedMessage(sender, new SetEconomiesMessage { EconomyMessages = messages });
        }

        void Handle_SetEconomies(TDNetworkConnection sender, Stream stream)
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

        void Handle_SetEconomy(TDNetworkConnection sender, Stream stream)
        {
            var message = MessagePackSerializer.Deserialize<SetEconomyMessage>(stream);

            var economy = GetEconomy(message.PlayerId);

            economy.currency = message.Currency;
            eventManager.onEconomyChanged.Raise(message.PlayerId);
        }
    }
}