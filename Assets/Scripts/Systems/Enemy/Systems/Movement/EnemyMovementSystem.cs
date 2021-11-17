using TDGame.Systems.Enemy.Components.Movement;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

// ReSharper disable AccessToDisposedClosure

namespace TDGame.Systems.Enemy.Systems.Movement
{
    public class EnemyMovementSystem : SystemBase
    {
        public float3[] path;

        EntityQuery moveQuery;
        protected override void OnCreate()
        {
           moveQuery  = GetEntityQuery(new EntityQueryDesc
           {
               All = new ComponentType[] { typeof(EnemyMoveTowards) },
           });
          
        }

        protected override void OnUpdate()
        {
            if (path == null)
                return;
            
            var waypoints = new NativeArray<float3>(path, Allocator.TempJob);
            var commandBuffer = new EntityCommandBuffer(Allocator.TempJob);

            var job = new EnemyMovementJob()
            {
                CommandBuffer = commandBuffer.AsParallelWriter(),
                DeltaTime = Time.DeltaTime,
                Waypoints = waypoints,
                EntityType = GetEntityTypeHandle(),
                MoveTowardsHandle = GetComponentTypeHandle<EnemyMoveTowards>(),
                TranslationHandle = GetComponentTypeHandle<Translation>()
            };

            job.ScheduleParallel(moveQuery, Dependency).Complete();

            commandBuffer.Playback(EntityManager);
            commandBuffer.Dispose();
            waypoints.Dispose();
        }
    }

    [BurstCompatible]
    struct EnemyMovementJob : IJobChunk
    {
        public EntityCommandBuffer.ParallelWriter CommandBuffer;

        [ReadOnly]
        public float DeltaTime;

        [ReadOnly]
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