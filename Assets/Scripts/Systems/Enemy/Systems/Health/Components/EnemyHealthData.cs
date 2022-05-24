using Unity.Entities;

namespace TDGame.Systems.Enemy.Systems.Health.Components
{
    [GenerateAuthoringComponent]
    public struct EnemyHealthData : IComponentData
    {
        public int Health;
        public int MaxHealth;
    }
}