using TDGame.Systems.Stats.Implementations.Movement;
using Unity.Entities;
using UnityEngine;

namespace TDGame.Systems.Stats.Implementations.Range
{
    public class BaseRangeStatAuthoring : MonoBehaviour
    {
        public float Value;

        public class BaseMovementSpeedStatBaker : Baker<BaseRangeStatAuthoring>
        {
            public override void Bake(BaseRangeStatAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new BaseMovementSpeedStat() { Value = authoring.Value });
            }
        }
    }
}