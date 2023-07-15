using MessagePack;
using Unity.Entities;
using Unity.Mathematics;

namespace TDGame.Systems.Building.Messages.Server
{
    /// <summary>
    /// Sent from Server to Client
    /// </summary>
    [MessagePackObject]
    public struct RemoveBuildingMessage
    {
        [Key(0)]
        public int Id;
    }
}