using Unity.Entities;
using UnityEngine;

namespace TDGame.Systems.Tower.Attack.Implementations.Projectile.Components
{
    public struct ProjectileLifetime : IComponentData
    {
        public float Value;
    }
    public class ProjectileLifetimeAuthoring : MonoBehaviour
    {
        public int Value;
    }

    public class ProjectileLifetimeBaker : Baker<ProjectileLifetimeAuthoring>
    {
        public override void Bake(ProjectileLifetimeAuthoring authoring)
        {
            AddComponent(new ProjectileLifetime { Value = authoring.Value });
        }
    }
}