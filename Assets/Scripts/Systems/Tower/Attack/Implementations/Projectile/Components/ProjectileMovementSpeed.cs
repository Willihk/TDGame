using Unity.Entities;
using Unity.Mathematics;

namespace TDGame.Systems.Tower.Attack.Implementations.Projectile.Components
{
    [GenerateAuthoringComponent]
    public struct ProjectileMovementSpeed : IComponentData
    {
        public float Value;
    }
}