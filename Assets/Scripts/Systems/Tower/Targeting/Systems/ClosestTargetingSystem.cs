using TDGame.Systems.Enemy.Components;
using TDGame.Systems.Tower.Targeting.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace TDGame.Systems.Tower.Targeting.Systems
{
    public partial class ClosestTargetingSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem commandBufferSystem;

        private EntityQuery enemyQuery;
        private EntityQuery towerQuery;

        protected override void OnCreate()
        {
            commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

            enemyQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(EnemyTag), typeof(Translation) }
            });
            towerQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(TowerTag), typeof(RequestEnemyTargetTag), typeof(TargetRange), typeof(TargetBufferElement)
                }
            });
            
        }

        protected override void OnUpdate()
        {
            var enemies = enemyQuery.ToEntityArrayAsync(Allocator.TempJob, out var enemyHandle);

            var tra = GetComponentDataFromEntity<Translation>(true);
            
            var job = new TargetJob
            {
                CommandBuffer = commandBufferSystem.CreateCommandBuffer().AsParallelWriter(),
                EnemyTranslations = tra,
                EnemyEntities = enemies,
                TranslationHandle = GetComponentTypeHandle<Translation>(),
                RangeHandle = GetComponentTypeHandle<TargetRange>(),
                targetBufferHandle = GetBufferTypeHandle<TargetBufferElement>(),
                EntityHandle = GetEntityTypeHandle()
            };
            var handle = job.ScheduleParallel(towerQuery, enemyHandle);
            commandBufferSystem.AddJobHandleForProducer(handle);
            Dependency = JobHandle.CombineDependencies(Dependency, handle);
        }


        [BurstCompile]
        private struct TargetJob : IJobEntityBatch
        {
            public EntityCommandBuffer.ParallelWriter CommandBuffer;

            public BufferTypeHandle<TargetBufferElement> targetBufferHandle;

            [ReadOnly]
            public ComponentDataFromEntity<Translation> EnemyTranslations;

            [ReadOnly]
            [DeallocateOnJobCompletion]
            public NativeArray<Entity> EnemyEntities;

            [ReadOnly]
            public ComponentTypeHandle<Translation> TranslationHandle;

            [ReadOnly]
            public ComponentTypeHandle<TargetRange> RangeHandle;

            [ReadOnly]
            public EntityTypeHandle EntityHandle;

            public void Execute(ArchetypeChunk batchInChunk, int batchIndex)
            {
                var translations = batchInChunk.GetNativeArray(TranslationHandle);
                var ranges = batchInChunk.GetNativeArray(RangeHandle);
                var entities = batchInChunk.GetNativeArray(EntityHandle);

                var buffers = batchInChunk.GetBufferAccessor(targetBufferHandle);

                for (int i = 0; i < batchInChunk.Count; i++)
                {
                    float closestRange = float.MaxValue;
                    var closest = Entity.Null;

                    for (int j = 0; j < EnemyEntities.Length; j++)
                    {
                        float distance = math.distance(translations[i].Value, EnemyTranslations[EnemyEntities[j]].Value);

                        if (ranges[i].Range < distance || distance > closestRange)
                            continue;

                        closestRange = distance;
                        closest = EnemyEntities[j];
                    }
                    
                    if (closest == Entity.Null)
                        continue;

                    if (buffers[i].Length == 0)
                    {
                        buffers[i].Add(closest);
                    }
                    else
                    {
                        var buffer = buffers[i];
                        buffer[0] = closest;
                    }
                    // else
                    // {
                    //     var buffer = CommandBuffer.AddBuffer<TargetBufferElement>(batchIndex, entities[i]);
                    //
                    //     buffer.Add(closest);
                    // }
                }
            }
        }
    }
}