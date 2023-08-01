using TDGame.Network.Components.DOTS;
using TDGame.PrefabManagement;
using TDGame.Systems.Enemy.Components;
using TDGame.Systems.Enemy.Components.Spawning;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

namespace TDGame.Systems.Enemy.Systems.Spawning
{
    public partial class SimpleEnemySpawningSystem : SystemBase
    {
        EndSimulationEntityCommandBufferSystem entityCommandBufferSystem;
        
        protected override void OnCreate()
        {
            RequireForUpdate<EnemySpawnPoint>();
            RequireForUpdate<PrefabManagerTag>();
            entityCommandBufferSystem = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            if (!SystemAPI.TryGetSingleton(out EnemySpawnPoint spawnPoint))
                return;
            
            var commandBuffer = entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            var prefabManagerEntity = SystemAPI.GetSingletonEntity<PrefabManagerTag>();
            var buffer = SystemAPI.GetBuffer<PrefabElement>(prefabManagerEntity);
            

            Entities.WithReadOnly(buffer).WithNone<NetworkSend>().ForEach((Entity entity, int entityInQueryIndex, in SpawnEnemy spawnEnemy) =>
            {
                foreach (var item in buffer)
                {
                    if (item.GUID != spawnEnemy.prefab)
                        continue;

                    var newEnemy = commandBuffer.Instantiate(entityInQueryIndex, item.Value);
                    
                    commandBuffer.SetComponent(entityInQueryIndex,newEnemy, LocalTransform.FromPosition(spawnPoint.Value));
                    
                    commandBuffer.AddComponent<EnemyTag>(entityInQueryIndex, newEnemy);
                }


                commandBuffer.DestroyEntity(entityInQueryIndex, entity);
            }).ScheduleParallel();
            
            entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}