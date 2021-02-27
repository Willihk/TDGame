using UnityEngine;
using Mirror;
using TDGame.Network.Player;
using System.Collections.Generic;
using TDGame.Building;
using TDGame.Enemy.Data;
using TDGame.Network.EventBinding;
using TDGame.Network.Lobby;
using UnityEngine.SceneManagement;

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

        public Dictionary<NetworkConnection, int> connectionRelations = new Dictionary<NetworkConnection, int>();

        public override GameObject OnRoomServerCreateRoomPlayer(NetworkConnection conn)
        {
            var playerData = new PlayerData
                {Name = "Name " + connectionRelations[conn], Id = connectionRelations[conn]};

            GameObject gameobject = Instantiate(roomPlayerPrefab.gameObject);
            var lobbyPlayer = gameobject.GetComponent<NetworkedLobbyPlayer>();
            lobbyPlayer.Setup(playerData);


            connectedPlayers.Add(connectionRelations[conn], playerData);
            eventBinder.ServerOnClientConnect(conn);

            return gameobject;
        }

        public override void OnRoomServerConnect(NetworkConnection conn)
        {
            eventBinder.ServerOnClientConnect(conn);
            int id = Random.Range(0, int.MaxValue);
            connectionRelations.Add(conn, id);
        }

        public override void OnRoomServerDisconnect(NetworkConnection conn)
        {
            if (connectedPlayers.ContainsKey(connectionRelations[conn]))
                connectedPlayers.Remove(connectionRelations[conn]);

            connectionRelations.Remove(conn);

            eventBinder.ServerOnClientDisconnect(conn);
        }

        public override void OnRoomServerPlayersReady()
        {
        }

        public void GotoGameScene()
        {
            if (NetworkServer.active)
            {
                ServerChangeScene(GameplayScene);
            }
        }

        void RemoveFromDontDestroyOnLoad()
        {
            if (gameObject.scene.name == "DontDestroyOnLoad" && !string.IsNullOrEmpty(offlineScene) &&
                SceneManager.GetActiveScene().path != offlineScene)
                SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
        }

        public override void OnRoomStopClient()
        {
            RemoveFromDontDestroyOnLoad();
            connectedPlayers.Clear();
            connectionRelations.Clear();
        }

        public override void OnRoomStopServer()
        {
            RemoveFromDontDestroyOnLoad();
            connectedPlayers.Clear();
            connectionRelations.Clear();
        }
    }
}