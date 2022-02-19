using TDGame.Systems.Enemy.Components;
using TDGame.Systems.Enemy.Components.Spawning;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace TDGame.Systems.Enemy.Systems.Spawning
{
    public class SimpleEnemySpawningSystem : SystemBase
    {
        BeginSimulationEntityCommandBufferSystem entityCommandBufferSystem;

        EntityQuery spawnEnemyQuery;

        protected override void OnCreate()
        {
            entityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
            spawnEnemyQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(SpawnEnemy) },
            });
        }

        protected override void OnUpdate()
        {
            var commandBuffer = entityCommandBufferSystem.CreateCommandBuffer();
            var job = new SpawnEnemiesJob()
            {
                CommandBuffer = commandBuffer.AsParallelWriter(),
                EntityType = GetEntityTypeHandle(),
                SpawnEnemyType = GetComponentTypeHandle<SpawnEnemy>(true)
            };

            var handle = job.Schedule(spawnEnemyQuery);
            
            handle.Complete();

            entityCommandBufferSystem.AddJobHandleForProducer(JobHandle.CombineDependencies(Dependency, handle));
        }
    }

    [BurstCompatible]
    struct SpawnEnemiesJob : IJobChunk
    {
        public EntityCommandBuffer.ParallelWriter CommandBuffer;

        [ReadOnly]
        public EntityTypeHandle EntityType;

        [ReadOnly]
        public ComponentTypeHandle<SpawnEnemy> SpawnEnemyType;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            var entities = chunk.GetNativeArray(EntityType);
            var spawnEnemies = chunk.GetNativeArray(SpawnEnemyType);

            for (int i = 0; i < chunk.Count; i++)
            {
                var newEnemy = CommandBuffer.Instantiate(chunkIndex, spawnEnemies[i].prefab);
                CommandBuffer.RemoveComponent<Prefab>(chunkIndex, newEnemy);
                CommandBuffer.RemoveComponent<Disabled>(chunkIndex, newEnemy);

                CommandBuffer.AddComponent<EnemyTag>(chunkIndex, newEnemy);

                CommandBuffer.DestroyEntity(chunkIndex, entities[i]);
            }
        }
    }
}