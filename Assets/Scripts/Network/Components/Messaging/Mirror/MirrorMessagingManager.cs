using MessagePack;
using Mirror;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace TDGame.Network.Components.Messaging.Mirror
{
    public class MirrorMessagingManager : BaseMessagingManager
    {
        private void Awake()
        {
            Instance = this;

            NetworkServer.RegisterHandler<GenericMessage>(HandleGenericMessage);
            NetworkClient.RegisterHandler<GenericMessage>(HandleGenericMessage);
        }

        [SerializeField]
        Dictionary<string, NamedMessageDelegate> registeredCallbacks = new Dictionary<string, NamedMessageDelegate>();

        public override void RegisterNamedMessageHandler<T>(NamedMessageDelegate callback)
        {
            RegisterNamedMessageHandler(typeof(T).Name, callback);
        }

        public override void RegisterNamedMessageHandler(string name, NamedMessageDelegate callback)
        {
            registeredCallbacks.Add(name, callback);
        }

        public override void SendNamedMessage<T>(NetworkConnection target, T message)
        {
            SendNamedMessage(typeof(T).Name, target, message);
        }
        
        public override void SendNamedMessage<T>(string name, NetworkConnection target, T message)
        {
            var data = MessagePackSerializer.Serialize(message);
            var finalMessage = ConvertToMessage(name, data);
            NetworkServer.connections.First(x => x.Key == (int)target.id).Value.Send(finalMessage);
        }

        public override void SendNamedMessageToAll<T>(T message)
        {
            SendNamedMessageToAll(typeof(T).Name, message);
        }

        public override void SendNamedMessageToAll<T>(string name, T message)
        {
            var data = MessagePackSerializer.Serialize(message);
            var finalMessage = ConvertToMessage(name, data);
            NetworkServer.SendToAll(finalMessage);
        }

        private void HandleGenericMessage(global::Mirror.NetworkConnection sender, GenericMessage genericMessage)
        {
            Debug.Log("Received message: " + genericMessage.Name);
            if (registeredCallbacks.TryGetValue(genericMessage.Name, out NamedMessageDelegate callback))
            {
                var stream = new MemoryStream(genericMessage.Message);
                callback.Invoke(new NetworkConnection() { id = (ulong)sender.connectionId }, stream);
            }
        }

        private GenericMessage ConvertToMessage(string name, byte[] message)
        {
            Debug.Log("converting message: " + name);
            return new GenericMessage { Name = name, Message = message, };
        }

        public override void SendNamedMessageToServer<T>(T message)
        {
            SendNamedMessageToServer(typeof(T).Name, message);
        }

        public override void SendNamedMessageToServer<T>(string name, T message)
        {
            var data = MessagePackSerializer.Serialize(message);
            var finalMessage = ConvertToMessage(name, data);
            NetworkClient.Send(finalMessage);
        }
    }
}
