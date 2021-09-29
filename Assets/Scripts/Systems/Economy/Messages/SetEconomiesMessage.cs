using MessagePack;

namespace TDGame.Systems.Economy.Messages
{
    [MessagePackObject]
    public struct SetEconomiesMessage
    {
        [Key(0)]
        public SetEconomyMessage[] EconomyMessages;
    }
}