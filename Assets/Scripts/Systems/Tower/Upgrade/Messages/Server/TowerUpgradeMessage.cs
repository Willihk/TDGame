using MessagePack;
using UnityEngine;

namespace TDGame.Systems.Tower.Upgrade.Messages.Server
{
    [MessagePackObject]
    public struct TowerUpgradeMessage
    {
        [Key(0)]
        public int TowerId;

        [Key(0)]
        public Hash128 UpgradeHash;
    }
}