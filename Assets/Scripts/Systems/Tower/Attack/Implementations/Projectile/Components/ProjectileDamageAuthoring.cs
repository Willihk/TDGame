using Unity.Entities;
using UnityEngine;

namespace TDGame.Systems.Tower.Attack.Implementations.Projectile.Components
{
    public struct ProjectileDamage : IComponentData
    {
        public int Value;
    }

    public class ProjectileDamageAuthoring : MonoBehaviour
    {
        public int Value;
    }

    public class ProjectileDamageBaker : Baker<ProjectileDamageAuthoring>
    {
        public override void Bake(ProjectileDamageAuthoring authoring)
        {
            AddComponent(new ProjectileDamage { Value = authoring.Value });
        }
    }
}