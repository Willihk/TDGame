using Unity.Entities;

namespace TDGame.Systems.Stats.Implementations.Range
{
    public struct BaseRangeStat : IComponentData, IBaseStat
    {
        public float value;

        public float Value
        {
            readonly get => value;
            set => this.value = value;
        }
    }
}