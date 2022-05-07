using Unity.Entities;

namespace TDGame.Systems.Tower.Attack.Implementations.Projectile
{
    [GenerateAuthoringComponent]
    public struct ShotPrefab : IComponentData
    {
        public Entity value;
    }
}