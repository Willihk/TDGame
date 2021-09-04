﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using MessagePack;
using Mirror;
using Sirenix.OdinInspector;
using TDGame.Network.Components.Messaging;
using TDGame.Network.Messages.Player;
using TDGame.Network.Player;
using UnityEngine;
using UnityEngine.Events;
using NetworkConnection = TDGame.Network.Components.Messaging.NetworkConnection;
using Random = UnityEngine.Random;

namespace TDGame.Network.Components
{
    public class NetworkPlayerManager : MonoBehaviour
    {
        // Called on both server & client 
        public UnityEvent<int> onPlayerRegistered;

        // Called on both server & client 
        public UnityEvent onPlayersSynced;

        // Called on both server & client 
        public UnityEvent<int> onPlayerUnregistered;

        [SerializeField]
        private PlayerList playerList;

        private void Awake()
        {
            onPlayerRegistered ??= new UnityEvent<int>();
            onPlayerUnregistered ??= new UnityEvent<int>();
        }


        // Key is INetworkPlayer, value is id assigned to the player
        // This is only on the server
        [Sirenix.OdinInspector.ShowInInspector]
        private Dictionary<ulong, int> registeredPlayers = new Dictionary<ulong, int>();

        [Sirenix.OdinInspector.ShowInInspector]
        [ReadOnly]
        private HashSet<ulong> connections = new HashSet<ulong>();

        BaseMessagingManager messagingManager;

        void Start()
        {
            messagingManager = BaseMessagingManager.Instance;

            messagingManager.RegisterNamedMessageHandler<RegisterPlayerData>(Handle_ClientRegistrationMessage);

            messagingManager.RegisterNamedMessageHandler<PlayerRegistered>(Handle_PlayerRegistered);
            messagingManager.RegisterNamedMessageHandler<PlayerUnregistered>(Handle_PlayerUnregistered);
            messagingManager.RegisterNamedMessageHandler<SetPlayerList>(Handle_SetPlayerList);
        }

        public void OnServerStarted()
        {
            playerList.Clear();
            registeredPlayers = new Dictionary<ulong, int>();
            connections = new HashSet<ulong>();
        }

        public void OnServerStopped()
        {
            registeredPlayers.Clear();
            connections.Clear();
        }


        // Called when a player has been registered by the server.
        // Called on all connected clients.
        private void Handle_PlayerRegistered(NetworkConnection sender, Stream stream)
        {
            var message = MessagePackSerializer.Deserialize<PlayerRegistered>(stream);
            onPlayerRegistered.Invoke(message.Id);
            Debug.Log("Player registered: " + message.Id);
        }

        // Called when a player has been unregistered/disconnected by the server.
        // Called on all connected clients.
        private void Handle_PlayerUnregistered(NetworkConnection sender, Stream stream)
        {
            var message = MessagePackSerializer.Deserialize<PlayerUnregistered>(stream);
            onPlayerUnregistered.Invoke(message.Id);
            Debug.Log("Player unregistered: " + message.Id);
        }


        private bool RegisterPlayer(ulong player)
        {
            if (registeredPlayers.ContainsKey(player))
                return false;

            int id = Random.Range(1, 2_000_000);

            registeredPlayers.Add(player, id);

            // This is sent to all other clients
            messagingManager.SendNamedMessageToAll(new PlayerRegistered { Id = id });

            // Needed since the server.SendToAll does not activate on the server/host
            //MemoryStream data = new MemoryStream(MessagePackSerializer.Serialize(new PlayerRegistered { Id = id }));
            //Handle_PlayerRegistered((networkManager.LocalClientId), data);


            return true;
        }

        public void Server_OnClientDisconnected(ulong player)
        {

            if (registeredPlayers.TryGetValue(player, out int id))
            {
                // This is sent to all other clients
                messagingManager.SendNamedMessageToAll(new PlayerUnregistered { Id = id });

                // Needed since the server.SendToAll does not activate on the server/host
                //MemoryStream data = new MemoryStream(MessagePackSerializer.Serialize(new PlayerRegistered { Id = id }));
                //Handle_PlayerRegistered(networkManager.LocalClientId, data);
            }

            registeredPlayers.Remove(player);
            connections.Remove(player);
        }

        public void Server_OnClientConnected(ulong player)
        {
            // Only run on server
            connections.Add(player);

            UniTask.Create(async () =>
            {
                await UniTask.Delay(100);
                messagingManager.SendNamedMessage(new NetworkConnection() { id = player }, new SetPlayerList { Players = registeredPlayers.Values.ToList() });
            });
        }

        public void Handle_ClientRegistrationMessage(NetworkConnection sender, Stream stream)
        {
            if (!NetworkServer.active)
                return;

            RegisterPlayer(sender.id);
        }


        #region Client Specific

        public void Client_OnConnected(ulong player)
        {
            async UniTaskVoid SendRegistrationMessage()
            {
                await UniTask.Delay(100);
                messagingManager.SendNamedMessageToServer(new RegisterPlayerData() { Name = "player" });
            }

            SendRegistrationMessage().Forget();
        }

        private void Handle_SetPlayerList(NetworkConnection sender, Stream stream)
        {
            var message = MessagePackSerializer.Deserialize<SetPlayerList>(stream);
            playerList.players = message.Players;
            onPlayersSynced.Invoke();
        }

        public void Client_Disconnected()
        {
            playerList.Clear();
        }

        #endregion
    }
}