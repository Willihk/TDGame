using System;
using Sirenix.OdinInspector;
using Unity.Networking.Transport;
using UnityEngine;

namespace TDGame.Network.Components
{
    public class TransportClientWrapper : MonoBehaviour
    {
        private NetworkDriver driver;
        private NetworkConnection connection;

        public ushort port = 7777;

        [ReadOnly]
        public bool isConnected;

        private void Start()
        {
            driver = NetworkDriver.Create();
           
        }

        [Button]
        public void ConnectToLocalhost()
        {
            var endpoint = NetworkEndPoint.LoopbackIpv4; // localhost
            endpoint.Port = port;

            connection = driver.Connect(endpoint);
        } 
        [Button]
        public void Disconnect()
        {
            connection.Disconnect(driver);
        }

        private void OnDestroy()
        {
            driver.Dispose();
            isConnected = false;
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
                        break;
                    case NetworkEvent.Type.Disconnect:
                        isConnected = false;
                        connection = default;
                        break;
                    case NetworkEvent.Type.Data:
                        break;
                    default:
                        // Should never reach
                        break;
                }
            }
        }
    }
}