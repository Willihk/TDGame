using TDGame.Systems.Enemy.Components;
using TDGame.Systems.Stats.Implementations.Range;
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
    public partial class OutOfRangeSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem commandBufferSystem;

        private EntityQuery enemyQuery;
        private EntityQuery towerQuery;

        protected override void OnCreate()
        {
            commandBufferSystem = World.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();

            enemyQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(EnemyTag), typeof(LocalTransform) }
            });
            towerQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(TowerTag), typeof(RequestEnemyTargets), typeof(FinalRangeStat), typeof(TargetBufferElement)
                }
            });
        }

        protected override void OnUpdate()
        {
            var translations = GetComponentLookup<LocalTransform>(true);

            var job = new OutOfRangeJob
            {
                targetBufferHandle = GetBufferTypeHandle<TargetBufferElement>(),
                EnemyTranslations = translations,
                TranslationHandle = GetComponentTypeHandle<LocalTransform>(),
                RangeHandle = GetComponentTypeHandle<FinalRangeStat>(),
                EntityHandle = GetEntityTypeHandle()
            };

            var handle = job.Schedule(towerQuery, Dependency);
            commandBufferSystem.AddJobHandleForProducer(handle);
            Dependency = JobHandle.CombineDependencies(Dependency, handle);
        }

        [BurstCompile]
        private struct OutOfRangeJob : IJobChunk
        {
            public BufferTypeHandle<TargetBufferElement> targetBufferHandle;

            [ReadOnly]
            public ComponentLookup<LocalTransform> EnemyTranslations;

            [ReadOnly]
            public ComponentTypeHandle<LocalTransform> TranslationHandle;

            [ReadOnly]
            public ComponentTypeHandle<FinalRangeStat> RangeHandle;

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
                    var buffer = buffers[i];
                    for (int j = 0; j < buffer.Length; j++)
                    {
                        if (EnemyTranslations.HasComponent(buffer[j]))
                        {
                            float distance = math.distance(translations[i].Position,
                                EnemyTranslations[buffer[j]].Position);

                            if (distance > ranges[i].Value)
                                buffer.RemoveAt(j);
                        }
                        else
                        {
                            buffer.RemoveAt(j);
                        }
                    }
                }
            }
        }
    }
}