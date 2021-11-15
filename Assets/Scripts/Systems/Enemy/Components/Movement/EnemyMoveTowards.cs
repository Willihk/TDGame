using Unity.Entities;

namespace TDGame.Systems.Enemy.Components.Movement
{
    [GenerateAuthoringComponent]
    public struct EnemyMoveTowards : IComponentData
    {
        public float Speed;
        public int waypointIndex;
    }
}