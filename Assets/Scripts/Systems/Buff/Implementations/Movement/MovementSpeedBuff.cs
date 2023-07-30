using Unity.Entities;

namespace TDGame.Systems.Buff.Implementations.Movement
{
    public struct MovementSpeedBuff : IBaseBuff, IComponentData
    {
        public float Value { get; set; }
        public float Duration { get; set; }
        public StatModifierType ModifierType { get; set; }
        public int Stacks { get; set; }
        public int AppliedBy {get; set;}
    }
}