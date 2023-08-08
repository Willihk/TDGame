using Unity.Entities;

namespace TDGame.Systems.Stats.Implementations.Movement
{
    public struct FinalMovementSpeedStat : IComponentData, IBaseStat
    {
        public float value;

        public float Value
        {
            readonly get => value;
            set => this.value = value;
        }
    }
}