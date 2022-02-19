using Unity.Entities;

namespace TDGame.Systems.Tower.Targeting.Components
{
    [GenerateAuthoringComponent]
    public struct TargetRange : IComponentData
    {
        public int Range;
    }
}