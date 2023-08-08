using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TDGame.Systems.Tower.Attack.Implementations.Projectile.Components
{
    public struct ProjectileFiringPoint : IComponentData
    {
        public Entity firingPoint;
    }

    public class ProjectileFiringPointAuthoring : MonoBehaviour
    {
        public GameObject FiringPoint;

        public class ProjectileFiringPointBaker : Baker<ProjectileFiringPointAuthoring>
        {
            public override void Bake(ProjectileFiringPointAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity,
                    new ProjectileFiringPoint
                        {
                            firingPoint = GetEntity(authoring.FiringPoint, TransformUsageFlags.Dynamic)
                        });
            }
        }
    }
}