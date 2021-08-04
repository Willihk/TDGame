using System;
using System.Collections.Generic;

namespace TDGame.Network.Messages.Player
{
    [Serializable]
    public struct SetPlayerList
    {
        public List<int> Players;
    }
}