using TDGame.Systems.Stats.Implementations.Movement;
using Unity.Entities;
using UnityEngine;

namespace TDGame.Systems.Stats.Implementations.Damage
{
    public class BaseDamageStatAuthoring : MonoBehaviour
    {
        public float Value;

        public class BaseMovementSpeedStatBaker : Baker<BaseDamageStatAuthoring>
        {
            public override void Bake(BaseDamageStatAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new BaseMovementSpeedStat() { Value = authoring.Value });
            }
        }
    }
}