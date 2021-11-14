using System.Collections.Generic;
using TDGame.Systems.Enemy.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
// ReSharper disable AccessToDisposedClosure

namespace TDGame.Systems.Enemy.Systems
{
    public class EnemyMovementSystem : SystemBase
    {
        public float3[] path;

        protected override void OnUpdate()
        {
            var waypoints = new NativeArray<float3>(path, Allocator.TempJob);
            var deltaTime = Time.DeltaTime;
            
            Entities.ForEach((ref Translation translation, ref MoveForward moveForward) =>
            {
                if (moveForward.waypointIndex >= waypoints.Length)
                {
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

            }).Run();

            waypoints.Dispose();
        }
    }
}