using Unity.Entities;
using UnityEngine;

namespace TDGame.Systems.Tower
{
    public struct TowerTag : IComponentData
    {
    }
    
    public class TowerTagAuthoring : MonoBehaviour
    {
        class Baker : Baker<TowerTagAuthoring>
        {
            public override void Bake(TowerTagAuthoring authoring)
            {
                AddComponent<TowerTag>(GetEntity(TransformUsageFlags.None));
            }
        }
    }
}