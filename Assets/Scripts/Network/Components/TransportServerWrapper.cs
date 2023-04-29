using System;
using Sirenix.OdinInspector;
using TDGame.Network.Components.DOTS;
using TDGame.Network.Components.Messaging;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.Events;

namespace TDGame.Network.Components
{
    public enum MessageType : byte
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
        public UnityEvent<TDNetworkConnection> connectionAccepted;
        public UnityEvent<TDNetworkConnection> connectionClosed;

        public Action<TDNetworkConnection, byte[]> onReceivedData;

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
                var data = new NativeArray<byte>(payload.Length + 1, Allocator.Temp);
                
                data[0] = (byte)MessageType.Managed;
                var dataSpan = data.AsSpan().Slice(1);
                payload.CopyTo(dataSpan);
                
                // Debug.Log("Server sending data to all with length: " + data.Length);
                // Debug.Log("data sent to all: " + String.Join("," ,data));

                driver.BeginSend(pipeline, connections[i], out var writer);
                writer.WriteBytes(data);
                driver.EndSend(writer);
            }
        }

        public void SendToAllEntities(Span<byte> payload)
        {
            for (int i = 0; i < connections.Length; i++)
            {
                var data = new NativeArray<byte>(payload.Length + 1, Allocator.Temp);
                
                data[0] = (byte)MessageType.Entities;
                var dataSpan = data.AsSpan().Slice(1);
                payload.CopyTo(dataSpan);
                
                // Debug.Log("Server sending data to all with length: " + data.Length);
                // Debug.Log("data sent to all: " + String.Join("," ,data));
                

                driver.BeginSend(pipeline, connections[i], out var writer);
                writer.WriteBytes(data);
                driver.EndSend(writer);
            }
        }

        public void Send(NetworkConnection connection, Span<byte> payload)
        {
            if (connection == default)
            {
                Debug.LogError("Trying to send a message to default connection");
                return;
            }

            var data = new NativeArray<byte>(payload.Length + 1, Allocator.Temp);

            data[0] = (byte)MessageType.Managed;
            var dataSpan = data.AsSpan().Slice(1);
            payload.CopyTo(dataSpan);
            
            Debug.Log("Server sending data with length: " + data.Length);
            Debug.Log("Server data sent: " + String.Join(",", data));

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
                connectionAccepted.Invoke(new TDNetworkConnection {NetworkConnection = conn});
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
                            connectionClosed.Invoke(new TDNetworkConnection {NetworkConnection = connections[i]});
                            connections[i] = default;
                            break;
                        case NetworkEvent.Type.Data:
                            // data
                            var nativeArray = new NativeArray<byte>(reader.Length, Allocator.Temp);
                            reader.ReadBytes(nativeArray);

                            switch (nativeArray[0])
                            {
                                case (byte)MessageType.Managed:
                                    var data = nativeArray.AsReadOnlySpan().Slice(1).ToArray();

                                    nativeArray.Dispose();
                                    onReceivedData?.Invoke(new TDNetworkConnection {NetworkConnection = connections[i]}, data);
                                    break; 
                                
                                case (byte)MessageType.Entities:
                                    var nativeData = new NativeArray<byte>(reader.Length - 1, Allocator.TempJob);
                                    nativeArray.Slice(1).CopyTo(nativeData);

                                    World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<ReceiveNetworkComponents>().ReceiveData(nativeData);
                                    break;
                                default:
                                    break;
                            }

                            break;
                    }
                }
            }
        }
    }
}