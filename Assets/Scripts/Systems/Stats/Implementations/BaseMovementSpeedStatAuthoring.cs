using Unity.Entities;
using UnityEngine;

namespace TDGame.Systems.Stats.Implementations
{
    public class BaseMovementSpeedStatAuthoring : MonoBehaviour
    {
        public float Value;

        public class BaseMovementSpeedStatBaker : Baker<BaseMovementSpeedStatAuthoring>
        {
            public override void Bake(BaseMovementSpeedStatAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new BaseMovementSpeedStat() { Value = authoring.Value });
            }
        }
    }
}