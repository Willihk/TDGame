using TDGame.PrefabManagement;
using TDGame.Systems.Enemy.Components;
using TDGame.Systems.Enemy.Components.Spawning;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace TDGame.Systems.Enemy.Systems.Spawning
{
    public partial class SimpleEnemySpawningSystem : SystemBase
    {
        BeginSimulationEntityCommandBufferSystem entityCommandBufferSystem;
        
        protected override void OnCreate()
        {
            entityCommandBufferSystem = World.GetExistingSystemManaged<BeginSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var commandBuffer = entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            var prefabManagerEntity = SystemAPI.GetSingletonEntity<PrefabManagerTag>();
            var buffer = SystemAPI.GetBuffer<PrefabElement>(prefabManagerEntity);
            

            Entities.WithReadOnly(buffer).ForEach((Entity entity, int entityInQueryIndex, in SpawnEnemy spawnEnemy) =>
            {
                var newEnemy = commandBuffer.Instantiate(entityInQueryIndex, buffer[0].Value);
                commandBuffer.AddComponent<EnemyTag>(entityInQueryIndex, newEnemy);

                commandBuffer.DestroyEntity(entityInQueryIndex, entity);
            }).ScheduleParallel();
            
            entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }

    // [GenerateTestsForBurstCompatibility]
    // struct SpawnEnemiesJob : IJobChunk
    // {
    //     public EntityCommandBuffer.ParallelWriter CommandBuffer;
    //
    //     [ReadOnly]
    //     public EntityTypeHandle EntityType;
    //
    //     [ReadOnly]
    //     public ComponentTypeHandle<SpawnEnemy> SpawnEnemyType;
    //
    //     public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
    //     {
    //         var entities = chunk.GetNativeArray(EntityType);
    //         var spawnEnemies = chunk.GetNativeArray(SpawnEnemyType);
    //
    //         for (int i = 0; i < chunk.Count; i++)
    //         {
    //             var newEnemy = CommandBuffer.Instantiate(chunkIndex, spawnEnemies[i].prefab);
    //             CommandBuffer.RemoveComponent<Prefab>(chunkIndex, newEnemy);
    //             CommandBuffer.RemoveComponent<Disabled>(chunkIndex, newEnemy);
    //
    //             CommandBuffer.AddComponent<EnemyTag>(chunkIndex, newEnemy);
    //
    //             CommandBuffer.DestroyEntity(chunkIndex, entities[i]);
    //         }
    //     }
    // }
}