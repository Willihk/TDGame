using MessagePack;

namespace TDGame.Systems.Building.Messages.Server
{
    /// <summary>
    /// Sent from Server to Client
    /// </summary>
    [MessagePackObject]
    public struct RemovePlacementMessage
    {
        [Key(0)]
        public int PlayerId;
    }
}