﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TDGame.Network.Player
{
    [Obsolete]
    public class InGamePlayerManager : NetworkBehaviour
    {
        [SerializeField]
        private GameObject playerPrefab;

        private Dictionary<int, GameObject> playerObjects;

        private Dictionary<int, GameObject> freePlayerObjects;

        private void Awake()
        {
            playerObjects = new Dictionary<int, GameObject>();
            freePlayerObjects = new Dictionary<int, GameObject>();
        }

        public GameObject[] GetPlayerObjects()
        {
            return playerObjects.Values.ToArray();
        }

        public override void OnStartServer()
        {
            foreach (var connection in NetworkServer.connections.Values)
            {
                PlayerConnected(connection);
            }
        }

        public void PlayerConnected(NetworkConnection connection)
        {
            print("Player Connected");
            if (freePlayerObjects.Count == 0)
            {
                CreateNewPlayerObject(connection);
                var pair = freePlayerObjects.First();

                freePlayerObjects.Remove(pair.Key);
                playerObjects.Add(pair.Key, pair.Value);
            }
        }

        public void PlayerDisconnected(NetworkConnection connection)
        {
            int id = GetIdByConnection(connection);
            playerObjects.Remove(id);
            freePlayerObjects.Remove(id);
        }

        void CreateNewPlayerObject(NetworkConnection connection)
        {
            print("creating player object");
            int id = GetIdByConnection(connection);

            var playerObject = Instantiate(playerPrefab);
            var player = playerObject.GetComponent<PlayerNetworkController>();

            var playerData = TDGameNetworkManager.Instance.connectedPlayers[GetIdByConnection(connection)];
            player.Initialize(id);
            player.Setup(playerData.Name);
            NetworkServer.Spawn(playerObject, connection);
            freePlayerObjects.Add(id, playerObject);
        }

        void AssignPlayerToConnection(NetworkConnection connection, int playerId)
        {
            print("assigning player object");
            var playerObject = GetPlayerById(playerId);
            var identity = playerObject.GetComponent<NetworkIdentity>();
            identity.AssignClientAuthority(connection);
        }

        public GameObject GetPlayerById(int id)
        {
            playerObjects.TryGetValue(id, out GameObject playerObject);
            return playerObject;
        }

        public int GetIdByConnection(NetworkConnection connection)
        {
            if (TDGameNetworkManager.Instance.connectionRelations.ContainsKey(connection))
                return TDGameNetworkManager.Instance.connectionRelations[connection];

            return -1;
        }
    }
}