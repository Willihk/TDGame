using MessagePack;

namespace TDGame.Systems.Building.Messages.Server
{
    /// <summary>
    /// Sent from Server to Client
    /// </summary>
    [MessagePackObject]
    public struct NewPlacementMessage
    {
        [Key(0)]
        public string AssetGuid;

        [Key(1)]
        public int PlayerId;
    }
}