using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Mirage;
using Sirenix.OdinInspector;
using TDGame.Network.Messages.Player;
using TDGame.Network.Player;
using UnityEngine;
using UnityEngine.Events;
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

        [SerializeField]
        private NetworkServer server;

        // Key is INetworkPlayer, value is id assigned to the player
        // This is only on the server
        [Sirenix.OdinInspector.ShowInInspector]
        private Dictionary<INetworkPlayer, int> registeredPlayers = new Dictionary<INetworkPlayer, int>();

        [Sirenix.OdinInspector.ShowInInspector]
        [ReadOnly]
        private HashSet<INetworkPlayer> connections = new HashSet<INetworkPlayer>();


        public void OnHostStarted()
        {
           
        }

        public void OnHostStopped()
        {
            
        }


        public void OnServerStarted()
        {
            playerList.Clear();
            registeredPlayers = new Dictionary<INetworkPlayer, int>();
            connections = new HashSet<INetworkPlayer>();
        }

        public void OnServerStopped()
        {
            registeredPlayers.Clear();
            connections.Clear();
        }

        // Called when a player has been registered by the server.
        // Called on all connected clients.
        private void Handle_PlayerRegistered(INetworkPlayer sender, PlayerRegistered message)
        {
            onPlayerRegistered.Invoke(message.Id);
            Debug.Log("Player registered: " + message.Id);
        }

        // Called when a player has been unregistered/disconnected by the server.
        // Called on all connected clients.
        private void Handle_PlayerUnregistered(INetworkPlayer sender, PlayerUnregistered message)
        {
            onPlayerUnregistered.Invoke(message.Id);
            Debug.Log("Player unregistered: " + message.Id);
        }


        private bool RegisterPlayer(INetworkPlayer player)
        {
            if (registeredPlayers.ContainsKey(player))
                return false;

            int id = Random.Range(1, 2_000_000);

            registeredPlayers.Add(player, id);

            // This is sent to all other clients
            server.SendToAll(new PlayerRegistered { Id = id });

            // Needed since the server.SendToAll does not activate on the server/host
            Handle_PlayerRegistered(server.LocalPlayer, new PlayerRegistered { Id = id });


            return true;
        }

        public void Server_OnClientDisconnected(INetworkPlayer player)
        {
            if (server.Active)
            {
                player.UnregisterHandler<PlayerData>();

                if (registeredPlayers.TryGetValue(player, out int id))
                {
                    // This is sent to all other clients
                    server.SendToAll(new PlayerUnregistered { Id = id });

                    // Needed since the server.SendToAll does not activate on the server/host
                    Handle_PlayerRegistered(server.LocalPlayer, new PlayerRegistered { Id = id });
                }

                registeredPlayers.Remove(player);
                connections.Remove(player);
            }
        }

        public void Server_OnClientConnected(INetworkPlayer player)
        {
            // Only run on server
            if (server.Active)
            {
                player.RegisterHandler<PlayerData>(HandleClientRegistrationMessage);
                connections.Add(player);
                
                UniTask.Create(async () =>
                {
                    await UniTask.Delay(100);
                    player.Send(new SetPlayerList {Players = registeredPlayers.Values.ToList()});
                });
            }
        }

        public void HandleClientRegistrationMessage(INetworkPlayer player, PlayerData data)
        {
            if (!server.Active)
                return;

            RegisterPlayer(player);
        }


        #region Client Specific

        public void Client_OnConnected(INetworkPlayer player)
        {

            player.RegisterHandler<PlayerRegistered>(Handle_PlayerRegistered);
            player.RegisterHandler<PlayerUnregistered>(Handle_PlayerUnregistered);
            
            player.RegisterHandler<SetPlayerList>(Handle_SetPlayerList);

            async UniTaskVoid SendRegistrationMessage()
            {
                await UniTask.Delay(100);
                player.Send(new PlayerData()
                {
                    Name = "player"
                });
            } 
           
            SendRegistrationMessage().Forget();
        }

        private void Handle_SetPlayerList(INetworkPlayer sender, SetPlayerList message)
        {
            playerList.players = message.Players;
            onPlayersSynced.Invoke();
        }

        public void Client_Disconnected(INetworkPlayer player)
        {
            playerList.Clear();

            player.UnregisterHandler<PlayerRegistered>();
        }

        #endregion
    }
}