using System;

namespace TDGame.Network.Messages.Player
{
    [Serializable]
    public struct PlayerData
    {
        public string Name;
        public int Id;
    }
}