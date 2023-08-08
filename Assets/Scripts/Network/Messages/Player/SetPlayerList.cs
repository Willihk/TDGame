using MessagePack;
using System;
using System.Collections.Generic;

namespace TDGame.Network.Messages.Player
{
    [Serializable]
    [MessagePackObject]
    public struct SetPlayerList
    {
        [Key(0)]
        public int[] Players;
    }
}