using MessagePack;
using System;

namespace TDGame.Network.Messages.Player
{
    [Serializable]
    [MessagePackObject]
    public struct RegisterPlayerData
    {
        [Key(0)]
        public string Name;
        [Key(1)]
        public int Id;
    }
}