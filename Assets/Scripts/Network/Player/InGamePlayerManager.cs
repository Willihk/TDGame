using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TDGame.Network.Player
{
    public class InGamePlayerManager : NetworkBehaviour
    {
        [SerializeField]
        private GameObject playerPrefab;
        
        private Dictionary<int, GameObject> playerObjects;
        
        private Dictionary<int, GameObject> freePlayerObjects;


        private Dictionary<int, NetworkConnection> connectionRelations;

        private void Awake()
        {
            playerObjects = new Dictionary<int, GameObject>();
            freePlayerObjects = new Dictionary<int, GameObject>();
            connectionRelations = new Dictionary<int, NetworkConnection>();
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
                connectionRelations.Add(pair.Key, connection);
            }
        }

        void CreateNewPlayerObject(NetworkConnection connection)
        {
            print("creating player object");
            int id = Random.Range(0, int.MaxValue);

            var playerObject = Instantiate(playerPrefab);
            var player = playerObject.GetComponent<PlayerNetworkController>();

            var playerData = TDGameNetworkManager.Instance.connectedPlayers[connection.connectionId];
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

        GameObject GetPlayerById(int id)
        {
             playerObjects.TryGetValue(id, out GameObject playerObject);
             return playerObject;
        }
    }
}