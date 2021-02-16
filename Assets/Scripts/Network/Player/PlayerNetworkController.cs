using Mirror;
using System;
using System.Collections;
using TDGame.Building;
using TDGame.Building.Placement;
using UnityEngine;

namespace TDGame.Network.Player
{
    public class PlayerNetworkController : NetworkBehaviour
    {
        [SyncVar]
        [SerializeField]
        string name;

        [SyncVar]
        [SerializeField]
        int connectionId;

        [SerializeField]
        private GameObject placementPrefab;

        public void Setup(PlayerData playerData)
        {
            name = playerData.Name;
        }

        public void SpawnPlacementForPrefab(string prefabName)
        {
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