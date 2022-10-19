using System;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.Events;

namespace TDGame.Network.Components
{
    public enum MessageType
    {
        Undefined = 0,
        Managed = 1,
        Unmanaged = 2,
        Entities = 4,
    }

    public class TransportServerWrapper : MonoBehaviour
    {
        private NetworkDriver driver;
        private NativeList<NetworkConnection> connections;
        private NetworkPipeline pipeline;

        public ushort port = 7777;
        public int maxPlayers = 16;

        [Sirenix.OdinInspector.ReadOnly]
        public bool isListening;


        public UnityEvent serverStarted;
        public UnityEvent serverStopped;
        public UnityEvent<int> connectionAccepted;
        public UnityEvent<int> connectionClosed;

        public Action<int, byte[]> onReceivedData;

        private void Start()
        {
            driver = NetworkDriver.Create();
            pipeline = driver.CreatePipeline(typeof(ReliableSequencedPipelineStage));
        }

        [Button]
        public void StartServer()
        {
            var endpoint = NetworkEndpoint.AnyIpv4;
            endpoint.Port = port;

            if (driver.Bind(endpoint) != 0)
                Debug.LogError("Failed to bind server to port: " + port);
            else
                driver.Listen();

            connections = new NativeList<NetworkConnection>(maxPlayers, Allocator.Persistent);
            isListening = true;
            serverStarted.Invoke();
        }

        [Button]
        public void StopServer()
        {
            OnDestroy();
        }

        private void OnDestroy()
        {
            driver.Dispose();
            if (connections.IsCreated)
            {
                connections.Dispose();
            }
            isListening = false;
            serverStopped.Invoke();
        }

        public void SendToAll(Span<byte> payload)
        {
            for (int i = 0; i < connections.Length; i++)
            {
                var data = new NativeArray<byte>(payload.ToArray(), Allocator.Temp);
                Debug.Log("Server sending data to all with length: " + data.Length);
                Debug.Log("data sent to all: " + String.Join("," ,data));

                driver.BeginSend(pipeline, connections[i], out var writer);
                writer.WriteBytes(data);
                driver.EndSend(writer);
            }
        }

        public void Send(int id, Span<byte> payload)
        {
            NetworkConnection connection = default;
            for (int i = 0; i < connections.Length; i++)
            {
                if (connections[i].InternalId == id)
                    connection = connections[i];
            }

            if (connection == default)
            {
                Debug.LogError("Trying to send a message to unknown id: " + id);
                return;
            }

            var data = new NativeArray<byte>(payload.ToArray(), Allocator.Temp);

            Debug.Log("Server sending data with length: " + data.Length);
            Debug.Log("Server data sent: " + String.Join("," ,data));

            driver.BeginSend(pipeline, connection, out var writer);
            writer.WriteBytes(data);
            driver.EndSend(writer);
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
                connectionAccepted.Invoke(conn.InternalId);
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
                            connectionClosed.Invoke(connections[i].InternalId);
                            connections[i] = default;
                            break;
                        case NetworkEvent.Type.Data:
                            // data
                            Debug.Log("Server received data with length: " + reader.Length);
                            var nativeArray = new NativeArray<byte>(reader.Length, Allocator.Temp);
                            reader.ReadBytes(nativeArray);
                            var data = nativeArray.ToArray();
                            Debug.Log("Server data received: " + String.Join("," ,data));
                            nativeArray.Dispose();
                            onReceivedData?.Invoke(connections[i].InternalId, data);
                            break;
                    }
                }
            }
        }
    }
}