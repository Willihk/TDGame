using MessagePack;
using Mirror;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TDGame.Network.Components.Messaging.Mirror;
using UnityEngine;

namespace TDGame.Network.Components.Messaging.CustomTransport
{
    public class CustomTransportMessagingManager : BaseMessagingManager
    {
        [SerializeField]
        private TransportClientWrapper clientWrapper;
        
        [SerializeField]
        private TransportServerWrapper serverWrapper;
        
        private void Awake()
        {
            Instance = this;

            serverWrapper.onReceivedData += HandleGenericMessage;
            clientWrapper.onReceivedData += data => HandleGenericMessage(0, data);
        }

        Dictionary<int, NamedMessageDelegate> registeredCallbacks = new Dictionary<int, NamedMessageDelegate>();

        public override void RegisterNamedMessageHandler<T>(NamedMessageDelegate callback)
        {
            RegisterNamedMessageHandler(typeof(T).Name, callback);
        }

        public override void RegisterNamedMessageHandler(string name, NamedMessageDelegate callback)
        {
            registeredCallbacks.Add(name.GetHashCode(), callback);
        }

        public override void SendNamedMessage<T>(NetworkConnection target, T message)
        {
            SendNamedMessage(typeof(T).Name, target, message);
        }
        
        public override void SendNamedMessage<T>(string name, NetworkConnection target, T message)
        {
            var finalMessage = ConvertToMessage(name, message);
            var finalData = MessagePackSerializer.Serialize(finalMessage);

            serverWrapper.Send(target.id, finalData);
        }

        public override void SendNamedMessageToAll<T>(T message)
        {
            SendNamedMessageToAll(typeof(T).Name, message);
        }

        public override void SendNamedMessageToAll<T>(string name, T message)
        {
            var finalMessage = ConvertToMessage(name, message);
            var data = MessagePackSerializer.Serialize(finalMessage);

            serverWrapper.SendToAll(data);
        }

        private void HandleGenericMessage(int sender, byte[] data)
        {
            var genericMessage = MessagePackSerializer.Deserialize<GenericMessage>(data);
            
            Debug.Log("Received message: " + genericMessage.Name);
            if (registeredCallbacks.TryGetValue(genericMessage.Name.GetHashCode(), out NamedMessageDelegate callback))
            {
                var stream = new MemoryStream(genericMessage.Message);
                callback.Invoke(new NetworkConnection { id = sender }, stream);
            }
        }

        private GenericMessage ConvertToMessage<T>(string name, T message)
        {
            var data = MessagePackSerializer.Serialize(message);
            Debug.Log("converting message: " + name);
            return new GenericMessage { Name = name.GetHashCode(), Message = data };
        }

        public override void SendNamedMessageToServer<T>(T message)
        {
            SendNamedMessageToServer(typeof(T).Name, message);
        }

        public override void SendNamedMessageToServer<T>(string name, T message)
        {
            var finalMessage = ConvertToMessage(name, message);

            var data = MessagePackSerializer.Serialize(finalMessage);
            
            clientWrapper.SendToServer(data);
        }
    }
}
