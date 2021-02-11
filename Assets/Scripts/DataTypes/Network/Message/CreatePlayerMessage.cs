using Mirror;

namespace TDGame.Network.Message
{
    public struct CreatePlayerMessage : NetworkMessage
    {
        public string Name;
    }
}
