using System;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

namespace TDGame.Network.Components
{
    public class TransportServerWrapper : MonoBehaviour
    {
        private NetworkDriver driver;
        private NativeList<NetworkConnection> connections;

        public ushort port = 7777;
        public int maxPlayers = 16;

        [Sirenix.OdinInspector.ReadOnly]
        public bool isListening;

        private void Start()
        {
            driver = NetworkDriver.Create();
        }

        [Button]
        public void StartServer()
        {
            var endpoint = NetworkEndPoint.AnyIpv4;
            endpoint.Port = port;

            if (driver.Bind(endpoint) != 0)
                Debug.LogError("Failed to bind server to port: " + port);
            else
                driver.Listen();

            connections = new NativeList<NetworkConnection>(maxPlayers, Allocator.Persistent);
            isListening = true;
        }

        [Button("Disconnect")]
        private void OnDestroy()
        {
            driver.Dispose();
            connections.Dispose();
            isListening = false;
        }

        private void Update()
        {
            if (!isListening)
                return;
            
            driver.ScheduleUpdate().Complete();

            for (int i = 0; i < connections.Length; i++)
            {
                if (connections[i].IsCreated)
                    continue;
                
                connections.RemoveAtSwapBack(i);
                i--;
            }

            NetworkConnection conn;
            while ((conn = driver.Accept()) != default(NetworkConnection))
            {
                connections.Add(conn);
                Debug.Log("Accepted a new connection");
            }

            DataStreamReader reader;
            for (int i = 0; i < connections.Length; i++)
            {
                NetworkEvent.Type type;
                while ((type = driver.PopEventForConnection(connections[i], out reader)) != NetworkEvent.Type.Empty)
                {
                    switch (type)
                    {
                        case NetworkEvent.Type.Disconnect:
                            Debug.Log("Client disconnected from server");
                            connections[i] = default;
                            break;
                        case NetworkEvent.Type.Data:
                            // data
                            break;
                    }
                }
            }
        }
    }
}