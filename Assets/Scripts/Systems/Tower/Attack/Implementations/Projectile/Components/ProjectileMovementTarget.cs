using Unity.Entities;
using Unity.Mathematics;

namespace TDGame.Systems.Tower.Attack.Implementations.Projectile.Components
{
    [GenerateAuthoringComponent]
    public struct ProjectileMovementTarget : IComponentData
    {
        public float3 Value;
        
    }
}