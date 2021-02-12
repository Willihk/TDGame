using System;

namespace TDGame.Network.Player
{
    [Serializable]
    public struct PlayerData
    {
        public int ConnectionId;
        public string Name;
    }
}