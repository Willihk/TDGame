using Unity.Entities;

namespace TDGame.Systems.Tower.Attack.Implementations.Projectile.Components
{
    [GenerateAuthoringComponent]
    public struct ProjectileDamage : IComponentData
    {
        public int Value;
        
    }
}