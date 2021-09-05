using Mirror;
using TDGame.Network.Manager;
using UnityEngine;
using UnityEngine.Events;

namespace TDGame.Network
{
    public class GameNetworkManager : NetworkManager
    {
        public static GameNetworkManager Instance;

        [Header("Server Events")]
        public UnityEvent onServerStarted;
        public UnityEvent onServerStopped;
        public UnityEvent<ulong> onServerClientConnected;
        public UnityEvent<ulong> onServerClientDisconnected;

        [Header("Client Events")]
        public UnityEvent onClientStarted;
        public UnityEvent onClientStopped;
        public UnityEvent<ulong> onClientConnected;
        public UnityEvent<ulong> onClientDisconnected;

        public override void Awake()
        {
            base.Awake();
            Instance = this;
        }

        

        public override void OnStartServer()
        {
            onServerStarted.Invoke();
        }

        public override void OnStopServer()
        {
            onServerStopped.Invoke();
        }

        public override void OnServerConnect(NetworkConnection conn)
        {
            onServerClientConnected.Invoke((ulong)conn.connectionId);
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            onServerClientDisconnected.Invoke((ulong)conn.connectionId);
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            onClientConnected.Invoke((ulong)conn.connectionId);
        }
        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            onClientDisconnected.Invoke((ulong)conn.connectionId);
        }
    }
}