using Mirror;

namespace TDGame.Network.Message.Player
{
    public struct CreatePlayerMessage : NetworkMessage
    {
        public string Name;
    }
}
