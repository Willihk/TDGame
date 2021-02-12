using Mirror;
using System;
using System.Collections;
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

        public void Setup(PlayerData playerData)
        {
            Name = playerData.Name;
            connectionId = playerData.ConnectionId;
        }
    }
}