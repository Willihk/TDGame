using Unity.Entities;
using UnityEngine;

namespace TDGame.Systems.Tower.Attack.Implementations.AoE.Components
{
    public struct AoERange : IComponentData
    {
        public int Range;
    }

    public class AoERangeAuthoring : MonoBehaviour
    {
        public int Range;

        public class AoERangeBaker : Baker<AoERangeAuthoring>
        {
            public override void Bake(AoERangeAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new AoERange { Range = authoring.Range });
            }
        }
    }
}