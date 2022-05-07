using TDGame.Systems.Enemy.Components.Movement;
using Unity.Burst;
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
            commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            query = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(EnemyMoveTowards) },
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
                DeltaTime = Time.DeltaTime,
                Waypoints = waypoints,
                EntityType = GetEntityTypeHandle(),
                MoveTowardsHandle = GetComponentTypeHandle<EnemyMoveTowards>(),
                TranslationHandle = GetComponentTypeHandle<Translation>()
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

        public ComponentTypeHandle<Translation> TranslationHandle;

        public ComponentTypeHandle<EnemyMoveTowards> MoveTowardsHandle;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            var entities = chunk.GetNativeArray(EntityType);
            var translations = chunk.GetNativeArray(TranslationHandle);
            var enemyMoveTowards = chunk.GetNativeArray(MoveTowardsHandle);

            for (int i = 0; i < chunk.Count; i++)
            {
                if (enemyMoveTowards[i].waypointIndex >= Waypoints.Length)
                {
                    CommandBuffer.AddComponent<ReachedEndTag>(chunkIndex, entities[i]);
                    return;
                }

                var destination = Waypoints[enemyMoveTowards[i].waypointIndex];
                var translation = translations[i];

                if (math.distance(translation.Value, destination) > 1)
                {
                    var direction = math.normalize(destination - translation.Value);
                    direction.y = 0;
                    translation.Value += direction * enemyMoveTowards[i].Speed * DeltaTime;
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