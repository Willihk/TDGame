using Unity.Entities;

namespace TDGame.Systems.Stats.Implementations
{
    public struct FinalMovementSpeedStat : IComponentData, IBaseStat
    {
        public float Value { get; set; }

    }
}