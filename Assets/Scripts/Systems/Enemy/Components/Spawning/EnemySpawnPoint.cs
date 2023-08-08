using Unity.Entities;
using Unity.Mathematics;

namespace TDGame.Systems.Enemy.Components.Spawning
{
    public struct EnemySpawnPoint : IComponentData
    {
        public float3 Value;
    }
}