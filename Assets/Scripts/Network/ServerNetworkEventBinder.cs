using System.Collections;
using System.Collections.Generic;
using Mirror;
using TDGame.Events;
using TDGame.Network.Player;
using UnityEngine;

namespace TDGame.Network
{
    public class ServerNetworkEventBinder : MonoBehaviour
    {
        [Header("Server Event Bindings")] [SerializeField]
        GameEvent serverOnClientConnectEvent;

        [SerializeField] 
        NetworkGameEvent serverOnClientDisconnectEvent;


        #region Server Events

        public void ServerOnClientDisconnect(NetworkConnection connection)
        {
            serverOnClientDisconnectEvent.Raise(connection);
        }

        public void ServerOnClientConnect()
        {
            serverOnClientConnectEvent.Raise();
        }

        #endregion

    }
}