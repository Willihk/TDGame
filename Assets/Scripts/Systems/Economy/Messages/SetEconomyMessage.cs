using MessagePack;

namespace TDGame.Systems.Economy.Messages
{
    [MessagePackObject]
    public struct SetEconomyMessage
    {
        [Key(0)]
        public int PlayerId;

        [Key(1)]
        public int Currency;
    }
}