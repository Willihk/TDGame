using Mirror;
using System.IO;

namespace TDGame.Network.Components.Messaging.Mirror
{
    public struct GenericMessage : NetworkMessage
    {
        public string Name;
        public byte[] Message;
    }
}
