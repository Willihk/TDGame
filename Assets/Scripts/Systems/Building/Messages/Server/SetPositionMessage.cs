using MessagePack;
using UnityEngine;

namespace TDGame.Systems.Building.Messages.Server
{
    [MessagePackObject]
    public struct SetPositionMessage
    {
        [Key(0)]
        public int Id;

        [Key(1)]
        public Vector3 Position;
    }
}