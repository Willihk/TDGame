using Unity.Entities;

namespace TDGame.Systems.Enemy.Components
{
    [GenerateAuthoringComponent]
    public struct MoveForward : IComponentData
    {
        public float Speed;
        public int waypointIndex;
    }
}