using Unity.Entities;
using UnityEngine;

namespace TDGame.Systems.Buff.Implementations.Movement
{
    public class MovementSpeedBuffAuthoring : MonoBehaviour
    {
        public float Value; 
        public float Duration; 
        public StatModifierType ModifierType; 
        public int Stacks; 
        public int AppliedBy;
        public class MovementSpeedBuffBaker : Baker<MovementSpeedBuffAuthoring>
        {
            public override void Bake(MovementSpeedBuffAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new MovementSpeedBuff()
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