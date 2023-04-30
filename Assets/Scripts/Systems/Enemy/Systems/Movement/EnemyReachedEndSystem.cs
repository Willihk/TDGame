using TDGame.Managers;
using TDGame.Systems.Enemy.Components.Movement;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace TDGame.Systems.Enemy.Systems.Movement
{
    public partial class EnemyReachedEndSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem commandBufferSystem;

        protected override void OnCreate()
        {
            commandBufferSystem = World.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var commandBuffer = commandBufferSystem.CreateCommandBuffer().AsParallelWriter();

            Entities.WithAll<ReachedEndTag>().ForEach((Entity entity, int entityInQueryIndex) =>
            {
                commandBuffer.DestroyEntity(entityInQueryIndex, entity);
                
                GameManager.Instance.EnemyReachedEnd();
            }).WithoutBurst().Run();

            commandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }

    // [BurstCompile]
    // struct RemoveEnemyJob : IJobEntityBatch
    // {
    //     public EntityCommandBuffer.ParallelWriter CommandBuffer;
    //
    //     [ReadOnly]
    //     public EntityTypeHandle EntityType;
    //
    //     public void Execute(ArchetypeChunk batchInChunk, int batchIndex)
    //     {
    //         var entities = batchInChunk.GetNativeArray(EntityType);
    //
    //         for (int i = 0; i < batchInChunk.Count; i++)
    //         {
    //             CommandBuffer.DestroyEntity(batchIndex, entities[i]);
    //         }
    //     }
    // }
}