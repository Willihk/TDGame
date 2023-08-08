using Unity.Entities;
using UnityEngine;

namespace TDGame.Systems.Buff.Implementations.Range
{
    public struct RangeBuff : IBaseBuff, IComponentData
    {
        [SerializeField]
        private float duration;
        [SerializeField]
        private float value;
        [SerializeField]
        private StatModifierType modifierType;
        [SerializeField]
        private int stacks;
        [SerializeField]
        private int appliedBy;

        public float Value
        {
            readonly get => value;
            set => this.value = value;
        }

        public float Duration
        {
            readonly get => duration;
            set => duration = value;
        }

        public StatModifierType ModifierType
        {
            readonly get => modifierType;
            set => modifierType = value;
        }

        public int Stacks
        {
            readonly get => stacks;
            set => stacks = value;
        }

        public int AppliedBy
        {
            readonly get => appliedBy;
            set => appliedBy = value;
        }
    }
}