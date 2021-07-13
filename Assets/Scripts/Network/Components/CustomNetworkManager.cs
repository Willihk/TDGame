using System;
using Mirage;
using UnityEngine;

namespace TDGame.Network.Components
{
    public class CustomNetworkManager : MonoBehaviour
    {
        // TODO: Remove singleton
        public static CustomNetworkManager Instance { get; private set; }

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
    }
}