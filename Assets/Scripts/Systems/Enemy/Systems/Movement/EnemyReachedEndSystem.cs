using TDGame.Systems.Enemy.Components.Movement;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace TDGame.Systems.Enemy.Systems.Movement
{
    public partial class EnemyReachedEndSystem : SystemBase
    {
        EntityQuery query;

        protected override void OnCreate()
        {
            query = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(ReachedEndTag) },
            });
        }

        protected override void OnUpdate()
        {
            var commandBuffer = new EntityCommandBuffer(Allocator.TempJob);

            var job = new RemoveEnemyJob()
            {
                CommandBuffer = commandBuffer.AsParallelWriter(),
                EntityType = GetEntityTypeHandle(),
            };

            job.ScheduleParallel(query, 1, Dependency).Complete();

            commandBuffer.Playback(EntityManager);
            commandBuffer.Dispose();
        }
    }
    
    [BurstCompatible]
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