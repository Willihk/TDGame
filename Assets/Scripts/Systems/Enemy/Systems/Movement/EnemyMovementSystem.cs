using TDGame.Systems.Enemy.Components.Movement;
using TDGame.Systems.Stats.Implementations;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

// ReSharper disable AccessToDisposedClosure

namespace TDGame.Systems.Enemy.Systems.Movement
{
    public partial class EnemyMovementSystem : SystemBase
    {
        public float3[] path;

        EntityQuery query;

        private EndSimulationEntityCommandBufferSystem commandBufferSystem;

        protected override void OnCreate()
        {
            commandBufferSystem = World.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
            query = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(EnemyMoveTowards), typeof(FinalMovementSpeedStat) },
                None = new ComponentType[] { typeof(ReachedEndTag) }
            });
        }

        protected override void OnUpdate()
        {
            if (path == null)
                return;

            var waypoints = new NativeArray<float3>(path, Allocator.TempJob);

            var job = new EnemyMovementJob()
            {
                CommandBuffer = commandBufferSystem.CreateCommandBuffer().AsParallelWriter(),
                DeltaTime = SystemAPI.Time.DeltaTime,
                Waypoints = waypoints,
                EntityType = GetEntityTypeHandle(),
                MoveTowardsHandle = GetComponentTypeHandle<EnemyMoveTowards>(),
                TranslationHandle = GetComponentTypeHandle<LocalTransform>(),
                MoveSpeedHandle = GetComponentTypeHandle<FinalMovementSpeedStat>(true)
            };

            Dependency = job.ScheduleParallel(query, Dependency);
            commandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }

    [BurstCompile]
    struct EnemyMovementJob : IJobChunk
    {
        public EntityCommandBuffer.ParallelWriter CommandBuffer;

        [ReadOnly]
        public float DeltaTime;

        [ReadOnly]
        [DeallocateOnJobCompletion]
        public NativeArray<float3> Waypoints;

        [ReadOnly]
        public EntityTypeHandle EntityType;

        public ComponentTypeHandle<LocalTransform> TranslationHandle;

        public ComponentTypeHandle<EnemyMoveTowards> MoveTowardsHandle;
        
        [ReadOnly]
        public ComponentTypeHandle<FinalMovementSpeedStat> MoveSpeedHandle;

        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask,
            in v128 chunkEnabledMask)
        {
            var entities = chunk.GetNativeArray(EntityType);
            var translations = chunk.GetNativeArray(ref TranslationHandle);
            var enemyMoveTowards = chunk.GetNativeArray(ref MoveTowardsHandle);
            var moveSpeeds = chunk.GetNativeArray(ref MoveSpeedHandle);

            var enumerator = new ChunkEntityEnumerator(useEnabledMask, chunkEnabledMask, chunk.Count);
            while (enumerator.NextEntityIndex(out int i))
            {
                if (enemyMoveTowards[i].waypointIndex >= Waypoints.Length)
                {
                    CommandBuffer.AddComponent<ReachedEndTag>(unfilteredChunkIndex, entities[i]);
                    return;
                }

                var destination = Waypoints[enemyMoveTowards[i].waypointIndex];
                var translation = translations[i];

                if (math.distance(translation.Position, destination) > 1)
                {
                    var direction = math.normalize(destination - translation.Position);
                    direction.y = 0;
                    translation.Position += direction * moveSpeeds[i].Value * DeltaTime;
                    translations[i] = translation;
                }
                else
                {
                    var moveForward = enemyMoveTowards[i];
                    moveForward.waypointIndex++;
                    enemyMoveTowards[i] = moveForward;
                }
            }
        }
    }
}