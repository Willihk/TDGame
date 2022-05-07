using Unity.Entities;

namespace TDGame.Systems.Tower.Attack.Implementations.Projectile.Components
{
    [GenerateAuthoringComponent]
    public struct ProjectilePrefab : IComponentData
    {
        public Entity Value;
    }
}