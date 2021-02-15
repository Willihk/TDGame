using UnityEngine;
using Mirror;
using System.Linq;
using TDGame.Network.Player;
using TDGame.Events;
using System.Collections.Generic;
using TDGame.Building;
using TDGame.Network.EventBinding;
using TDGame.Network.Message.Player;
using UnityEngine.Serialization;

/*
	Documentation: https://mirror-networking.com/docs/Components/NetworkManager.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkManager.html
*/

namespace TDGame.Network
{
    public class TDGameNetworkManager : NetworkManager
    {
        public static TDGameNetworkManager Instance;

        public override void Awake()
        {
            base.Awake();
            Instance = this;

            spawnPrefabs.AddRange(networkedBuildingList.GetBuildings());
        }

        [SerializeField] private ServerNetworkEventBinder eventBinder;

        [SerializeField] private BuildingList networkedBuildingList;

        public Dictionary<int, PlayerData> connectedPlayers = new Dictionary<int, PlayerData>();

        /// <summary>
        /// This is invoked when a server is started - including when a host is started.
        /// </summary>
        public override void OnStartServer()
        {
            base.OnStartServer();

            NetworkServer.RegisterHandler<CreatePlayerMessage>(OnCreatePlayer);
        }

        /// <summary>
        /// Called on the client when connected to a server.
        /// <para>The default implementation of this function sets the client as ready and adds a player. Override the function to dictate what happens when the client connects.</para>
        /// </summary>
        /// <param name="conn">Connection to the server.</param>
        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);

            CreatePlayerMessage message = new CreatePlayerMessage {Name = "Player " + Random.Range(0, 10000)};

            conn.Send(message);
        }

        private void OnCreatePlayer(NetworkConnection conn, CreatePlayerMessage message)
        {
            GameObject gameobject = Instantiate(playerPrefab);

            var playerData = new PlayerData {Name = message.Name};

            PlayerNetworkController player = gameobject.GetComponent<PlayerNetworkController>();
            player.Setup(playerData);

            // if (!connectedPlayers.ContainsKey(conn.connectionId))
            connectedPlayers.Add(conn.connectionId, playerData);

            eventBinder.ServerOnClientConnect(conn);

            NetworkServer.AddPlayerForConnection(conn, gameobject);
        }

        /// <summary>
        /// Called on the server when a client disconnects.
        /// <para>This is called on the Server when a Client disconnects from the Server. Use an override to decide what should happen when a disconnection is detected.</para>
        /// </summary>
        /// <param name="conn">Connection from client.</param>
        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);
            if (connectedPlayers.ContainsKey(conn.connectionId))
                connectedPlayers.Remove(conn.connectionId);
            
            eventBinder.ServerOnClientDisconnect(conn);
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            connectedPlayers.Clear();
        }
    }
}