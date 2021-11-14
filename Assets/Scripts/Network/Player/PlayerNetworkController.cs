using Mirror;
using System;
using System.Collections;
using TDGame.Building;
using TDGame.Building.Placement;
using UnityEngine;

namespace TDGame.Network.Player
{
    [Obsolete]
    public class PlayerNetworkController : NetworkBehaviour
    {
        [SyncVar]
        [SerializeField]
        string name;

        int playerId;

        [SerializeField]
        private GameObject placementPrefab;

        public void Setup(string playerName)
        {
            name = playerName;
        }

        public void Initialize(int id)
        {
            playerId = id;
        }

        public void SpawnPlacementForPrefab(string prefabName)
        {
            if (hasAuthority)
                Cmd_SpawnPrefabPlacer(prefabName);
        }

        [Command]
        private void Cmd_SpawnPrefabPlacer(string prefabName)
        {
            var placerObject = Instantiate(placementPrefab);
            var placer = placerObject.GetComponent<NetworkedBuildingPlacer>();
            placer.Setup(prefabName);
            NetworkServer.Spawn(placerObject, connectionToClient);
        }
    }
}