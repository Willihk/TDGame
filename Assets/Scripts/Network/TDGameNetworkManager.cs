using UnityEngine;
using Mirror;
using TDGame.Network.Player;
using System.Collections.Generic;
using TDGame.Building;
using TDGame.Enemy.Data;
using TDGame.Network.EventBinding;
using TDGame.Network.Message.Player;

namespace TDGame.Network
{
    public class TDGameNetworkManager : NetworkRoomManager
    {
        public static TDGameNetworkManager Instance;

        public override void Awake()
        {
            base.Awake();
            Instance = this;

            spawnPrefabs.AddRange(networkedBuildingList.GetBuildings());
            spawnPrefabs.AddRange(networkedEnemyList.GetEnemies());
        }

        [SerializeField]
        private ServerNetworkEventBinder eventBinder;

        [SerializeField]
        private BuildingList networkedBuildingList;
        
        [SerializeField]
        private EnemyList networkedEnemyList;

        public Dictionary<int, PlayerData> connectedPlayers = new Dictionary<int, PlayerData>();

        public override GameObject OnRoomServerCreateRoomPlayer(NetworkConnection conn)
        {
            GameObject gameobject = Instantiate(roomPlayerPrefab.gameObject);

            var playerData = new PlayerData {Name = "Name"};

            connectedPlayers.Add(conn.connectionId, playerData);

            eventBinder.ServerOnClientConnect(conn);

            return gameobject;
        }

        public override void OnRoomServerPlayersReady()
        {
            ServerChangeScene(GameplayScene);
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