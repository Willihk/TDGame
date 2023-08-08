using MessagePack;
using Unity.Entities;

namespace TDGame.Systems.Tower.Upgrade.Messages.Client
{
    [MessagePackObject]
    public struct RequestTowerUpgrade
    {
        [Key(0)]
        public int TowerId;

        [Key(1)]
        public Hash128 UpgradeHash;
    }
}