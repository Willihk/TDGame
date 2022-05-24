using Unity.Entities;

namespace TDGame.Systems.Tower.Attack.Implementations.Projectile.Components
{
    [GenerateAuthoringComponent]
    public struct ProjectileRadiusCollider : IComponentData
    {
        public float Value;
    }
}