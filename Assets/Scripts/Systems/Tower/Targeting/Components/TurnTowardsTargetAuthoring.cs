using Unity.Entities;
using UnityEngine;

namespace TDGame.Systems.Tower.Targeting.Components
{
    public struct TurnTowardsTarget : IComponentData
    {
        public Entity TurnPoint;
        public float TurnSpeed;
    }

    public class TurnTowardsTargetAuthoring : MonoBehaviour
    {
        public GameObject TurnPoint;
        public float TurnSpeed;

        public class TurnTowardsTargetBaker : Baker<TurnTowardsTargetAuthoring>
        {
            public override void Bake(TurnTowardsTargetAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity,
                    new TurnTowardsTarget
                        {
                            TurnPoint = GetEntity(authoring.TurnPoint, TransformUsageFlags.Dynamic),
                            TurnSpeed = authoring.TurnSpeed
                        });
            }
        }
    }
}