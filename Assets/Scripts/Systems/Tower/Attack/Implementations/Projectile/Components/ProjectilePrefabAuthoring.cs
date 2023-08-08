using Unity.Entities;
using UnityEngine;

namespace TDGame.Systems.Tower.Attack.Implementations.Projectile.Components
{
    public struct ProjectilePrefab : IComponentData
    {
        public Entity Value;
    }
    public class ProjectilePrefabAuthoring : MonoBehaviour
    {
        public GameObject Prefab;
    }

    public class ProjectilePrefabBaker : Baker<ProjectilePrefabAuthoring>
    {
        public override void Bake(ProjectilePrefabAuthoring authoring)
        {
            AddComponent(GetEntity(TransformUsageFlags.Dynamic),new ProjectilePrefab { Value = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic) });
        }
    }
}