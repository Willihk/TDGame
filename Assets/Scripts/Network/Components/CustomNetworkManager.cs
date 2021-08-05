using System;
using Mirage;
using UnityEngine;

namespace TDGame.Network.Components
{
    public class CustomNetworkManager : MonoBehaviour
    {
        // TODO: Remove singleton
        public static CustomNetworkManager Instance { get; private set; }

        public NetworkManager mirageManager;

        [SerializeField]
        private NetworkClient client;

        [SerializeField]
        private NetworkServer server;


        private void Awake()
        {
            Instance = this;
        }

        public void ConnectToServer(string address, ushort port = 7777)
        {
            client.Connect(address, port);
        }

        public void StartServer()
        {
            server.StartServer(client);
        }

        public void StartHost()
        {
            server.StartServer();
        }

        /// <summary>
        /// Stops the server if running.
        /// Disconnects the local client if connected.
        /// </summary>
        public void Stop()
        {
            if (server.Active)
                server.Stop();
            else if (client.Active)
                client.Disconnect();
        }
    }
}