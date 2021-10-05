using MessagePack;

namespace TDGame.Systems.Building.Messages.Client
{
    [MessagePackObject]
    public struct StartPlacementMessage
    {
        [Key(0)]
        public string AssetGuid;
    }
}