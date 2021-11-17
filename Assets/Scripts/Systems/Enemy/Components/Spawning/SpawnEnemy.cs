using Unity.Entities;

namespace TDGame.Systems.Enemy.Components.Spawning
{
    public struct SpawnEnemy : IComponentData
    {
        public Entity prefab;
    }
}