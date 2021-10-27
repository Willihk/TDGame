using MessagePack;
using UnityEngine;

namespace TDGame.Systems.Building.Messages.Server
{
    /// <summary>
    /// Sent from Server to Client
    /// </summary>
    [MessagePackObject]
    public struct NewBuildingMessage
    {
        [Key(0)]
        public string AssetGuid;
        
        [Key(1)]
        public Vector3 Position;
    }
}