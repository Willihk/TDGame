using MessagePack;
using Unity.Entities;
using Unity.Mathematics;

namespace TDGame.Systems.Building.Messages.Server
{
    /// <summary>
    /// Sent from Server to Client
    /// </summary>
    [MessagePackObject]
    public struct NewBuildingMessage
    {
        [Key(0)]
        public Hash128 AssetGuid;
        
        [Key(1)]
        public float3 Position;

        [Key(2)]
        public int ID;
    }
}