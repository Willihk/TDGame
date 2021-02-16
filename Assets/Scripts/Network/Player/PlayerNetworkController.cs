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
        string Name;

        [SyncVar]
        [SerializeField]
        int connectionId;

        [SerializeField]
        private BuildingList networkedBuildingList;
        
        [SerializeField]
        private GameObject placementPrefab;

        public void Setup(PlayerData playerData)
        {
            Name = playerData.Name;
        }

        private void Update()
        {
            if (isLocalPlayer)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Cmd_SpawnTestCube();
                }
            }
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

        [Command]
        private void Cmd_SpawnTestCube()
        {
            var building = Instantiate(networkedBuildingList.GetBuilding(0));
            NetworkServer.Spawn(building, connectionToClient);
        }
    }
}