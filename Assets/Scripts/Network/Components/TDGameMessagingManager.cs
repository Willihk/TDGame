using MessagePack;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.Transports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MLAPI.Messaging.CustomMessagingManager;

namespace TDGame.Network.Components
{
    public class TDGameMessagingManager
    {


        public static void RegisterNamedMessageHandler<T>(HandleNamedMessageDelegate callback)
        {
            CustomMessagingManager.RegisterNamedMessageHandler(nameof(T), callback);
        }

        public static void SendNamedMessage<T>(ulong clientId, T message, NetworkChannel networkChannel = NetworkChannel.Internal)
        {
            MemoryStream data = new MemoryStream(MessagePackSerializer.Serialize(message));
            CustomMessagingManager.SendNamedMessage(nameof(T), clientId, data, networkChannel);
        }

        public static void SendNamedMessage(string name, ulong clientId, Stream stream, NetworkChannel networkChannel = NetworkChannel.Internal)
        {
            CustomMessagingManager.SendNamedMessage(name, clientId, stream, networkChannel);
        }
        public static void SendNamedMessageToAll<T>(T message, NetworkChannel networkChannel = NetworkChannel.Internal)
        {
            MemoryStream data = new MemoryStream(MessagePackSerializer.Serialize(message));
            SendNamedMessageToAll(nameof(T), data, networkChannel);
        }
        public static void SendNamedMessageToAll(string name, Stream stream, NetworkChannel networkChannel = NetworkChannel.Internal)
        {
            CustomMessagingManager.SendNamedMessage(name, NetworkManager.Singleton.ConnectedClients.Keys.ToList(), stream, networkChannel);
        }
    }
}
