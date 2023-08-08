using Unity.Entities;
using UnityEngine;

namespace TDGame.Systems.Tower.Attack.Implementations.Projectile.Components
{
    public struct ProjectileRadiusCollider : IComponentData
    {
        public float Value;
    }
    
    public class ProjectileRadiusColliderAuthoring : MonoBehaviour
    {
        public int Value;
    }

    public class ProjectileRadiusColliderBaker : Baker<ProjectileRadiusColliderAuthoring>
    {
        public override void Bake(ProjectileRadiusColliderAuthoring authoring)
        {
            AddComponent(new ProjectileRadiusCollider { Value = authoring.Value });
        }
    }
}