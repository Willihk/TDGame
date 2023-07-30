using TDGame.Systems.Buff.Implementations.Movement;
using Unity.Entities;
using UnityEngine;

namespace TDGame.Systems.Buff.Implementations.Range
{
    public class RangeBuffAuthoring : MonoBehaviour
    {
        public float Value; 
        public float Duration; 
        public StatModifierType ModifierType; 
        public int Stacks; 
        public int AppliedBy;
        public class RangeBuffBaker : Baker<RangeBuffAuthoring>
        {
            public override void Bake(RangeBuffAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new RangeBuff()
                {
                    Value = authoring.Value,
                    Duration = authoring.Duration,
                    AppliedBy = authoring.AppliedBy,
                    ModifierType = authoring.ModifierType,
                    Stacks = authoring.Stacks
                });
            }
        }
    }
}