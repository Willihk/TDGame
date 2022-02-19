using Unity.Entities;

namespace TDGame.Systems.Tower.Targeting.Components
{
    [GenerateAuthoringComponent]
    [InternalBufferCapacity(1)]
    public struct TargetBufferElement : IBufferElementData
    {
        public Entity Value;

        public static implicit operator Entity(TargetBufferElement e)
        {
            return e.Value;
        }

        public static implicit operator TargetBufferElement(Entity e)
        {
            return new TargetBufferElement { Value = e };
        }
    }
}