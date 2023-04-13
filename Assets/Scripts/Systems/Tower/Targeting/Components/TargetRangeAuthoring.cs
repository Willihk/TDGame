using TDGame.Systems.Enemy.Systems.Health.Components;
using Unity.Entities;
using UnityEngine;

namespace TDGame.Systems.Tower.Targeting.Components
{
    public struct TargetRange : IComponentData
    {
        public int Range;
    }

    public class TargetRangeAuthoring : MonoBehaviour
    {
        public int Range;
    }

    public class TargetRangeBaker : Baker<TargetRangeAuthoring>
    {
        public override void Bake(TargetRangeAuthoring authoring)
        {
            AddComponent(new TargetRange { Range = authoring.Range });
        }
    }
}