using Unity.Entities;
using UnityEngine;

namespace TDGame.Systems.Tower.Attack.Implementations.Projectile
{
    public struct ShotPrefab : IComponentData
    {
        public Entity value;
    }
    
    public class ShotPrefabAuthoring : MonoBehaviour
    {
        public GameObject Prefab;
    }

    public class ShotPrefabBaker : Baker<ShotPrefabAuthoring>
    {
        public override void Bake(ShotPrefabAuthoring authoring)
        {
            AddComponent(new ShotPrefab { value = GetEntity( authoring.Prefab)});
        }
    }
}