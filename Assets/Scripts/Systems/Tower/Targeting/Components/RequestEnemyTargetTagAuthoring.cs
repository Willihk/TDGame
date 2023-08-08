using Unity.Entities;
using UnityEngine;

namespace TDGame.Systems.Tower.Targeting.Components
{
    public struct RequestEnemyTargetTag : IComponentData
    {
    }

    public class RequestEnemyTargetTagAuthoring : MonoBehaviour
    {
        class Baker : Baker<RequestEnemyTargetTagAuthoring>
        {
            public override void Bake(RequestEnemyTargetTagAuthoring authoring)
            {
                AddComponent<RequestEnemyTargetTag>(GetEntity(TransformUsageFlags.Dynamic));
            }
        }
    }
}