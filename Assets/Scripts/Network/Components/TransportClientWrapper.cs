using System;
using Sirenix.OdinInspector;
using TDGame.Network.Components.DOTS;
using Unity.Collections;
using Unity.Entities;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.Events;

namespace TDGame.Network.Components
{
    public class TransportClientWrapper : MonoBehaviour
    {
        private NetworkDriver driver;
        private NetworkConnection connection;
        NetworkPipeline pipeline;

        public ushort port = 7777;

        public UnityEvent clientConnected;
        public UnityEvent clientDisconnected;
        public UnityEvent clientStopped;

        [Sirenix.OdinInspector.ReadOnly]
        public bool isConnected;

        public Action<byte[]> onReceivedData;
        

        private void Start()
        {
            driver = NetworkDriver.Create();
            pipeline = driver.CreatePipeline(typeof(ReliableSequencedPipelineStage));
        }

        [Button]
        public void ConnectToLocalhost()
        {
            var endpoint = NetworkEndpoint.LoopbackIpv4; // localhost
            endpoint.Port = port;

            connection = driver.Connect(endpoint);
        } 
        
        [Button]
        public void Connect(string ip)
        {
            var endpoint = NetworkEndpoint.Parse(ip, port);

            connection = driver.Connect(endpoint);
        } 
        
        [Button]
        public void Disconnect()
        {
            connection.Disconnect(driver);
            OnDestroy();
        }

        private void OnDestroy()
        {
            driver.Dispose();
            isConnected = false;
            clientStopped.Invoke();
        }

        public void SendToServer(Span<byte> payload)
        {
            var data = new NativeArray<byte>(payload.ToArray(), Allocator.Temp);
            Debug.Log("Client sending data with length: " + data.Length);
            Debug.Log("Client data sent to server: " + String.Join("," ,data));

            driver.BeginSend(pipeline, connection, out var writer);
            writer.WriteBytes(data);
            driver.EndSend(writer);
        }

        private void Update()
        {
            driver.ScheduleUpdate().Complete();

            if (!connection.IsCreated)
            {
                return;
            }

            DataStreamReader reader;
            NetworkEvent.Type type;

            while ((type = connection.PopEvent(driver, out reader))  != NetworkEvent.Type.Empty)
            {
                switch (type)
                {
                    case NetworkEvent.Type.Connect:
                        isConnected = true;
                        clientConnected.Invoke();
                        break;
                    case NetworkEvent.Type.Disconnect:
                        isConnected = false;
                        connection = default;
                        clientDisconnected.Invoke();
                        break;
                    case NetworkEvent.Type.Data:
                        Debug.Log("client received data with length: " + reader.Length);
                        var nativeArray = new NativeArray<byte>(reader.Length,Allocator.Temp);
                        switch (nativeArray[0])
                        {
                            case (byte)MessageType.Managed:
                                var data = nativeArray.ToArray();

                                Debug.Log("Server data received: " + String.Join(",", data));
                                nativeArray.Dispose();
                                onReceivedData?.Invoke(data);
                                break; 
                                
                            case (byte)MessageType.Entities:
                                World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<ReceiveNetworkComponents>().ReceiveData(nativeArray);
                                break;
                            default:
                                break;
                        }
                        break;
                    
                    default:
                        // Should never reach
                        break;
                }
            }
        }
    }
}