using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using MessagePack;
using Mirror;
using Sirenix.OdinInspector;
using TDGame.Network.Components.Messaging;
using TDGame.Network.Messages.Player;
using TDGame.Network.Player;
using TDGame.Player;
using UnityEngine;
using UnityEngine.Events;
using NetworkConnection = TDGame.Network.Components.Messaging.NetworkConnection;
using Random = UnityEngine.Random;

namespace TDGame.Network.Components
{
    public class NetworkPlayerManager : MonoBehaviour
    {
        public static NetworkPlayerManager Instance;

        // Called on both server & client 
        public UnityEvent<int> onPlayerRegistered;

        // Called on both server & client 
        public UnityEvent onPlayersSynced;

        // Called on both server & client 
        public UnityEvent<int> onPlayerUnregistered;

        [SerializeField]
        private PlayerList playerList;

        [SerializeField]
        private LocalPlayer localPlayer;

        private void Awake()
        {
            Instance = this;
            onPlayerRegistered ??= new UnityEvent<int>();
            onPlayerUnregistered ??= new UnityEvent<int>();
        }


        // Key is networkID, value is id assigned to the player
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

            messagingManager.RegisterNamedMessageHandler<SetLocalPlayer>(Handle_SetLocalPlayer);
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


        public int GetPlayerId(NetworkConnection connection)
        {
            return registeredPlayers[connection.id];
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


        private int RegisterPlayer(ulong player)
        {
            if (registeredPlayers.ContainsKey(player))
                return -1;

            int id = Random.Range(1, 2_000_000);

            registeredPlayers.Add(player, id);

            // This is sent to all other clients
            messagingManager.SendNamedMessageToAll(new PlayerRegistered { Id = id });

            // // Needed since the server.SendToAll does not activate on the server/host
            //MemoryStream data = new MemoryStream(MessagePackSerializer.Serialize(new PlayerRegistered { Id = id }));
            //Handle_PlayerRegistered((networkManager.LocalClientId), data);


            return id;
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
                messagingManager.SendNamedMessage(new NetworkConnection() { id = player },
                    new SetPlayerList { Players = registeredPlayers.Values.ToList() });
            });
        }

        public void Handle_ClientRegistrationMessage(NetworkConnection sender, Stream stream)
        {
            if (!NetworkServer.active)
                return;

            int id = RegisterPlayer(sender.id);

            messagingManager.SendNamedMessage(sender, new SetLocalPlayer { Id = id });
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
        
        private void Handle_SetLocalPlayer(NetworkConnection sender, Stream payload)
        {
            var message = MessagePackSerializer.Deserialize<SetLocalPlayer>(payload);
            localPlayer.SetupLocalPlayer(message.Id);
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