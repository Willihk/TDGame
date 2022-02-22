using TDGame.Systems.Enemy.Components;
using TDGame.Systems.Tower.Targeting.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace TDGame.Systems.Tower.Targeting.Systems
{
    public class OutOfRangeSystem : SystemBase
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
            var translations = GetComponentDataFromEntity<Translation>(true);

            var job = new OutOfRangeJob
            {
                targetBufferHandle = GetBufferTypeHandle<TargetBufferElement>(),
                EnemyTranslations = translations,
                TranslationHandle = GetComponentTypeHandle<Translation>(),
                RangeHandle = GetComponentTypeHandle<TargetRange>(),
                EntityHandle = GetEntityTypeHandle()
            };
            
            var handle = job.Schedule(towerQuery);
            commandBufferSystem.AddJobHandleForProducer(handle);
            Dependency = JobHandle.CombineDependencies(Dependency, handle);
        }

        [BurstCompatible]
        private struct OutOfRangeJob : IJobEntityBatch
        {
            public BufferTypeHandle<TargetBufferElement> targetBufferHandle;

            [ReadOnly]
            public ComponentDataFromEntity<Translation> EnemyTranslations;

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
                    var buffer = buffers[i];
                    for (int j = 0; j < buffer.Length; j++)
                    {
                        if (EnemyTranslations.HasComponent(buffer[j]))
                        {
                            float distance = math.distance(translations[i].Value, EnemyTranslations[buffer[j]].Value);
                            
                            if (distance > ranges[i].Range)
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