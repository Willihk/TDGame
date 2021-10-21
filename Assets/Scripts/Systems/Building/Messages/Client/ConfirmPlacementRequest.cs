using MessagePack;
using UnityEngine;

namespace TDGame.Systems.Building.Messages.Client
{
    [MessagePackObject]
    public struct ConfirmPlacementRequest
    {
        [Key(0)]
        public Vector3 Position;
    }
}