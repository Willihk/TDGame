using System.Collections.Generic;
using Mirage;
using Sirenix.OdinInspector;
using TDGame.Network.Player;
using UnityEngine;
using NetworkPlayer = Mirage.NetworkPlayer;

namespace TDGame.Network.Components
{
    public class NetworkPlayerManager : MonoBehaviour
    {
        [SerializeField]
        private NetworkServer server;
        private NetworkClient client;

        // Key is INetworkPlayer, value is id assigned to the player
        // This is only handled on the server
        [Sirenix.OdinInspector.ShowInInspector]
        private Dictionary<INetworkPlayer, int> registeredPlayers = new Dictionary<INetworkPlayer, int>();

        [Sirenix.OdinInspector.ShowInInspector]
        [ReadOnly]
        private HashSet<INetworkPlayer> connections = new HashSet<INetworkPlayer>();


        #region Server Specific

        #region Setup Server

        public void OnServerStarted()
        {
            registeredPlayers = new Dictionary<INetworkPlayer, int>();
            connections = new HashSet<INetworkPlayer>();
        }

        public void OnServerStopped()
        {
            registeredPlayers.Clear();
            connections.Clear();
        }

        #endregion


        private bool RegisterPlayer(INetworkPlayer player)
        {
            if (registeredPlayers.ContainsKey(player))
                return false;

            int id = Random.Range(1, 2_000_000);

            registeredPlayers.Add(player, id);

            return true;
        }

        public void Server_OnClientDisconnected(INetworkPlayer player)
        {
            if (server.Active)
            {
                player.UnregisterHandler<PlayerData>();
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
            }
        }

        public void HandleClientRegistrationMessage(INetworkPlayer player, PlayerData data)
        {
            if (!server.Active)
                return;

            RegisterPlayer(player);
        }

        #endregion

        #region Client Specific

        public void Client_OnConnected(INetworkPlayer player)
        {
            player.Send(new PlayerData()
            {
                Name = "player"
            });
        }

        #endregion
    }
}