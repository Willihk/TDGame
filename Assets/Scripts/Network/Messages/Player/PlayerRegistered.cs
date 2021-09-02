using MessagePack;

namespace TDGame.Network.Messages.Player
{
    [MessagePackObject]
    public struct PlayerRegistered
    {
        [Key(0)]
        public int Id;
    }
}