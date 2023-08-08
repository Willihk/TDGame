using Unity.Entities;
using Unity.Mathematics;

namespace TDGame.Systems.Tower.Attack.Implementations.Projectile.Components
{
    public struct ProjectileMovementDirection : IComponentData
    {
        public float3 Value;
    }
}