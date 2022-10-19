using Unity.Entities;

namespace TDGame.Systems.Enemy.Components.Spawning
{
    public struct SpawnEnemy : IComponentData
    {
        public Hash128 prefab;
    }
}