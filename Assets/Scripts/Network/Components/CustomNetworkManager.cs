
using System;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace TDGame.Network.Components
{
    public class CustomNetworkManager : MonoBehaviour
    {
        // TODO: Remove singleton
        public static CustomNetworkManager Instance { get; private set; }

        [SerializeField]
        public NetworkManager networkManager;

        private void Awake()
        {
            Instance = this;
        }

        public void StartServer()
        {
            networkManager.StartServer();
        }

        public void StartHost()
        {
            networkManager.StartHost();
        }

        public void StartClient(string ip, ushort port = 777)
        {
            networkManager.networkAddress = ip;
        }

        /// <summary>
        /// Stops the server if running.
        /// Disconnects the local client if connected.
        /// </summary>
        public void Stop()
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                // stop host if host-only
                networkManager.StopHost();
            }
            else if (NetworkClient.isConnected)
            {
                // stop client if client-only
                networkManager.StopClient();
            }
            else if (NetworkServer.active)
            {
                // stop server if server-only
                networkManager.StopServer();
            }
        }
    }
}