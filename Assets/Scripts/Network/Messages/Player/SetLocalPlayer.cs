using MessagePack;

namespace TDGame.Network.Messages.Player
{
    [MessagePackObject]
    public struct SetLocalPlayer
    {
        [Key(0)]
        public int Id;
    }
}