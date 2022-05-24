using Unity.Entities;

namespace TDGame.Systems.Tower.Attack.Implementations.Projectile.Components
{
    [GenerateAuthoringComponent]
    public struct ProjectileLifetime : IComponentData
    {
        public float Value;
    }
}