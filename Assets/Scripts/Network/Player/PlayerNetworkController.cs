using Mirror;
using System;
using System.Collections;
using TDGame.Building;
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

        [Command]
        void Cmd_SpawnTestCube()
        {
            var building = Instantiate(networkedBuildingList.GetBuilding(0));
            NetworkServer.Spawn(building, connectionToClient);
        }
    }
}