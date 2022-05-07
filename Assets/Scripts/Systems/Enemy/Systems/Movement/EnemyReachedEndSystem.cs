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
        
        EntityQuery query;

        protected override void OnCreate()
        {
            commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

            query = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(ReachedEndTag) },
            });
        }

        protected override void OnUpdate()
        {
            var job = new RemoveEnemyJob()
            {
                CommandBuffer = commandBufferSystem.CreateCommandBuffer().AsParallelWriter(),
                EntityType = GetEntityTypeHandle(),
            };

           Dependency = job.ScheduleParallel(query, Dependency);
           commandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
    
    [BurstCompile]
    struct RemoveEnemyJob : IJobEntityBatch
    {
        public EntityCommandBuffer.ParallelWriter CommandBuffer;

        [ReadOnly]
        public EntityTypeHandle EntityType;

        public void Execute(ArchetypeChunk batchInChunk, int batchIndex)
        {
            var entities = batchInChunk.GetNativeArray(EntityType);

            for (int i = 0; i < batchInChunk.Count; i++)
            {
                CommandBuffer.DestroyEntity(batchIndex, entities[i]);

            }
        }
    }
}