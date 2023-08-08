using MessagePack;
using Unity.Entities;

namespace TDGame.Systems.Tower.Upgrade.Messages.Server
{
    [MessagePackObject]
    public struct TowerUpgradeMessage
    {
        [Key(0)]
        public int TowerId;

        [Key(1)]
        public Hash128 UpgradeHash;
    }
}