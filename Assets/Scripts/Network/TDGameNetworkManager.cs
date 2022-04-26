using System;
using UnityEngine;
using Mirror;
using TDGame.Network.Player;
using System.Collections.Generic;
using TDGame.Building;
using TDGame.Data;
using TDGame.Network.EventBinding;
using TDGame.Network.Lobby;
using TDGame.Network.Messages.Player;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace TDGame.Network
{
    [Obsolete]
    public class TDGameNetworkManager : NetworkRoomManager
    {
        public static TDGameNetworkManager Instance;

        public override void Awake()
        {
            base.Awake();
            Instance = this;

            spawnPrefabs.AddRange(networkedBuildingList.GetGameObjects());
            spawnPrefabs.AddRange(networkedEnemyList.GetGameObjects());
        }

        [SerializeField]
        private ServerNetworkEventBinder eventBinder;

        [SerializeField]
        private Data.GameObjectList networkedBuildingList;

        [SerializeField]
        private GameObjectList networkedEnemyList;

        public Dictionary<int, RegisterPlayerData> connectedPlayers = new Dictionary<int, RegisterPlayerData>();

        public Dictionary<NetworkConnection, int> connectionRelations = new Dictionary<NetworkConnection, int>();

        //public override GameObject OnRoomServerCreateRoomPlayer(NetworkConnection conn)
        //{
        //    var playerData = new RegisterPlayerData
        //        {Name = "Name " + connectionRelations[conn], Id = connectionRelations[conn]};

        //    GameObject roomObject = Instantiate(roomPlayerPrefab.gameObject);
        //    var lobbyPlayer = roomObject.GetComponent<NetworkedLobbyPlayer>();
        //    lobbyPlayer.Setup(playerData);


        //    connectedPlayers.Add(connectionRelations[conn], playerData);
        //    eventBinder.ServerOnClientConnect(conn);
        //    return roomObject;
        //}

        //public override void OnRoomServerConnect(NetworkConnection conn)
        //{
        //    eventBinder.ServerOnClientConnect(conn);
        //    int id = Random.Range(0, int.MaxValue);
        //    connectionRelations.Add(conn, id);
        //}

        //public override void OnRoomServerDisconnect(NetworkConnection conn)
        //{
        //    eventBinder.ServerOnClientDisconnect(conn);
            
        //    if (connectedPlayers.ContainsKey(connectionRelations[conn]))
        //        connectedPlayers.Remove(connectionRelations[conn]);

        //    connectionRelations.Remove(conn);
        //}

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