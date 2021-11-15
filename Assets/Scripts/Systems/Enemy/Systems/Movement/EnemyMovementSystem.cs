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

        
        protected override void OnUpdate()
        {
            var waypoints = new NativeArray<float3>(path, Allocator.TempJob);
            var deltaTime = Time.DeltaTime;
            
            Entities.WithNone<ReachedEndTag>().ForEach((ref Entity entity,ref Translation translation, ref EnemyMoveTowards moveForward) =>
            {
                if (moveForward.waypointIndex >= waypoints.Length)
                {
                    EntityManager.AddComponent<ReachedEndTag>(entity);
                    return;
                }
                
                var destination = waypoints[moveForward.waypointIndex];
                if (math.distance(translation.Value, destination) > 1)
                {
                    var direction = math.normalize(destination - translation.Value);
                    direction.y = 0;
                    translation.Value += direction* moveForward.Speed * deltaTime;
                }
                else
                {
                    moveForward.waypointIndex++;
                }

            }).WithStructuralChanges().Run();

            
            waypoints.Dispose();
        }
    }
}