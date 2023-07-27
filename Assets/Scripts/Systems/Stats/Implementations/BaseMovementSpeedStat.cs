using Unity.Entities;
using UnityEngine;

namespace TDGame.Systems.Stats.Implementations
{
    public struct BaseMovementSpeedStat : IComponentData, IBaseStat
    {
        public float Value { get; set; }
    }
}