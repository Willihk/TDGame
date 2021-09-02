using MessagePack;

namespace TDGame.Network.Messages.Player
{
    [MessagePackObject]
    public struct PlayerUnregistered
    {
        [Key(0)]
        public int Id;
    }
}