using Unity.Entities;

namespace TDGame.Systems.Buff
{
    public interface IBaseBuff
    {
        public float Value { get; set; }
        public float Duration { get; set; }
        public StatModifierType ModifierType { get; set; }
        public int Stacks { get; set; }
        public int AppliedBy {get; set;}
    }
}