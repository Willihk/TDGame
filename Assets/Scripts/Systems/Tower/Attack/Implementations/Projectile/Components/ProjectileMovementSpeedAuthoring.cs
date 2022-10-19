using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TDGame.Systems.Tower.Attack.Implementations.Projectile.Components
{
    public struct ProjectileMovementSpeed : IComponentData
    {
        public float Value;
    }
    public class ProjectileMovementSpeedAuthoring : MonoBehaviour
    {
        public int Value;
    }

    public class ProjectileMovementSpeedBaker : Baker<ProjectileMovementSpeedAuthoring>
    {
        public override void Bake(ProjectileMovementSpeedAuthoring authoring)
        {
            AddComponent(new ProjectileMovementSpeed { Value = authoring.Value });
        }
    }
}