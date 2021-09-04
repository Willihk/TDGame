using Mirror;
using System.IO;

namespace TDGame.Network.Components.Messaging.Mirror
{
    public struct GenericMessage : NetworkMessage
    {
        public string Name;
        public string Type;
        public byte[] Message;
    }
}
