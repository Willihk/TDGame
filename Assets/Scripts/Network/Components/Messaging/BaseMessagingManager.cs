using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TDGame.Network.Components.Messaging
{
    public abstract class BaseMessagingManager : MonoBehaviour
    {
        public static BaseMessagingManager Instance { get; set; }

        public delegate void NamedMessageDelegate(NetworkConnection sender, Stream payload);

        public abstract void RegisterNamedMessageHandler<T>(NamedMessageDelegate callback);

        public abstract void RegisterNamedMessageHandler(string name, NamedMessageDelegate callback);


        public abstract void SendNamedMessage<T>(NetworkConnection target, T message) where T : struct;

        public abstract void SendNamedMessage<T>(string name, NetworkConnection target, T message) where T : struct;

        public abstract void SendNamedMessageToServer<T>(T message) where T : struct;

        public abstract void SendNamedMessageToServer<T>(string name, T message) where T : struct;


        public abstract void SendNamedMessageToAll<T>(T message) where T : struct;

        public abstract void SendNamedMessageToAll<T>(string name, T message) where T : struct;
    }
}
