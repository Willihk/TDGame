using Mirror;
using System.IO;
using MessagePack;

namespace TDGame.Network.Components.Messaging.CustomTransport
{
    [MessagePackObject]
    public struct GenericMessage
    {
        [Key(0)]
        public int Name;
        [Key(1)]
        public byte[] Message;
    }
}
