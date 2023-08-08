using System;
using UnityEngine;
using UnityEngine.Events;

namespace TDGame.Network.Components
{
    public class CustomNetworkManager : MonoBehaviour
    {
        // TODO: Remove singleton
        public static CustomNetworkManager Instance { get; private set; }

        [SerializeField]
        public TransportServerWrapper serverWrapper;
        
        [SerializeField]
        public TransportClientWrapper clientWrapper;

        private void Awake()
        {
            Instance = this;
        }

        public void StartServer()
        {
            serverWrapper.StartServer();
        }

        public void StartHost()
        {
            serverWrapper.StartServer();
            clientWrapper.ConnectToLocalhost();
        }

        public void StartClient(string ip, ushort port = 777)
        {
            clientWrapper.Connect(ip);
        }

        /// <summary>
        /// Stops the server if running.
        /// Disconnects the local client if connected.
        /// </summary>
        public void Stop()
        {
            if (serverWrapper.isListening && clientWrapper.isConnected)
            {
                // stop host if host-only
                clientWrapper.Disconnect();
                serverWrapper.StopServer();
            }
            else if (clientWrapper.isConnected)
            {
                // stop client if client-only
                clientWrapper.Disconnect();
            }
            else if (serverWrapper.isListening)
            {
                // stop server if server-only
                serverWrapper.StopServer();
            }
        }
    }
}