using Unity.Entities;
using UnityEngine;

namespace TDGame.Systems.Buff.Implementations.Damage
{
    public class DamageBuffAuthoring : MonoBehaviour
    {
        public float Value; 
        public float Duration; 
        public StatModifierType ModifierType; 
        public int Stacks; 
        public int AppliedBy;
        public class DamageBuffBaker : Baker<DamageBuffAuthoring>
        {
            public override void Bake(DamageBuffAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new DamageBuff()
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