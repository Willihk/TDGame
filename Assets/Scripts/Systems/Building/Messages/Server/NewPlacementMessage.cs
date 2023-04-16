using MessagePack;
using Unity.Entities;

namespace TDGame.Systems.Building.Messages.Server
{
    /// <summary>
    /// Sent from Server to Client
    /// </summary>
    [MessagePackObject]
    public struct NewPlacementMessage
    {
        [Key(0)]
        public Hash128 AssetGuid;

        [Key(1)]
        public int PlayerId;
    }
}