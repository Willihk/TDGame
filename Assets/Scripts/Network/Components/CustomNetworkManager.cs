using MLAPI;
using MLAPI.Transports.UNET;
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
        NetworkManager networkManager;


        UnityEvent<ulong> Client_OnClientConnected;
        UnityEvent<ulong> Client_OnClientDisconnected;


        UnityEvent onServerStarted;
        UnityEvent<ulong> Server_OnClientConnected;
        UnityEvent<ulong> Server_OnClientDisconnected;


        private void Awake()
        {
            Instance = this;
            onServerStarted ??= new UnityEvent();
        }

        private void Start()
        {
            networkManager.OnClientConnectedCallback += OnClientConnectedCallback;
            networkManager.OnClientDisconnectCallback += OnClientDisconnectCallback;
            networkManager.OnServerStarted += OnServerStarted;
        }

        private void OnClientDisconnectCallback(ulong id)
        {
            if (networkManager.IsServer)
            {
                Server_OnClientDisconnected.Invoke(id);
            }
            Client_OnClientDisconnected.Invoke(id);
        }

        private void OnClientConnectedCallback(ulong id)
        {
            if (networkManager.IsServer)
            {
                Server_OnClientConnected.Invoke(id);
            }
            Client_OnClientConnected.Invoke(id);
        }

        private void OnServerStarted()
        {
            onServerStarted.Invoke();
        }

        public void ConnectToServer(string address, ushort port = 7777)
        {
            networkManager.GetComponent<UNetTransport>().ConnectAddress = address;
            networkManager.GetComponent<UNetTransport>().ConnectPort = port;
            networkManager.StartClient();
        }

        public void StartServer()
        {
            networkManager.StartServer();
        }

        public void StartHost()
        {
            networkManager.StartHost();
        }

        /// <summary>
        /// Stops the server if running.
        /// Disconnects the local client if connected.
        /// </summary>
        public void Stop()
        {
            if (networkManager.IsHost)
                networkManager.StopHost();
            else if (networkManager.IsServer)
                networkManager.StopServer();
            else if (networkManager.IsClient)
                networkManager.StopClient();
        }
    }
}