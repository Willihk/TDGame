using TDGame.Systems.Enemy.Components;
using TDGame.Systems.Tower.Targeting.Components;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace TDGame.Systems.Tower.Targeting.Systems
{
    [RequireMatchingQueriesForUpdate]
    public partial class ClosestTargetingSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem commandBufferSystem;

        private EntityQuery enemyQuery;
        private EntityQuery towerQuery;

        protected override void OnCreate()
        {
            commandBufferSystem = World.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();

            enemyQuery = GetEntityQuery(ComponentType.ReadOnly<EnemyTag>(), ComponentType.ReadOnly<LocalTransform>());
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
            var enemies = enemyQuery.ToEntityArray(Allocator.TempJob);

            var transforms = GetComponentLookup<LocalTransform>(true);

            var job = new TargetJob
            {
                CommandBuffer = commandBufferSystem.CreateCommandBuffer().AsParallelWriter(),
                EnemyTranslations = transforms,
                EnemyEntities = enemies,
                TranslationHandle = GetComponentTypeHandle<LocalTransform>(),
                RangeHandle = GetComponentTypeHandle<TargetRange>(),
                targetBufferHandle = GetBufferTypeHandle<TargetBufferElement>(),
                EntityHandle = GetEntityTypeHandle()
            };
            var handle = job.ScheduleParallel(towerQuery, Dependency);
            commandBufferSystem.AddJobHandleForProducer(handle);
            Dependency = JobHandle.CombineDependencies(Dependency, handle);
        }


        [BurstCompile]
        private struct TargetJob : IJobChunk
        {
            public EntityCommandBuffer.ParallelWriter CommandBuffer;

            public BufferTypeHandle<TargetBufferElement> targetBufferHandle;

            [ReadOnly]
            public ComponentLookup<LocalTransform> EnemyTranslations;

            [ReadOnly]
            [DeallocateOnJobCompletion]
            public NativeArray<Entity> EnemyEntities;

            [ReadOnly]
            public ComponentTypeHandle<LocalTransform> TranslationHandle;

            [ReadOnly]
            public ComponentTypeHandle<TargetRange> RangeHandle;

            [ReadOnly]
            public EntityTypeHandle EntityHandle;

            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask,
                in v128 chunkEnabledMask)
            {
                var translations = chunk.GetNativeArray(ref TranslationHandle);
                var ranges = chunk.GetNativeArray(ref RangeHandle);
                var entities = chunk.GetNativeArray(EntityHandle);

                var buffers = chunk.GetBufferAccessor(ref targetBufferHandle);


                var enumerator = new ChunkEntityEnumerator(useEnabledMask, chunkEnabledMask, chunk.Count);

                while (enumerator.NextEntityIndex(out int i))
                {
                    float closestRange = float.MaxValue;
                    var closest = Entity.Null;

                    for (int j = 0; j < EnemyEntities.Length; j++)
                    {
                        float distance = math.distance(translations[i].Position,
                            EnemyTranslations[EnemyEntities[j]].Position);

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