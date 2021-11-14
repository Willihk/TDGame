using System.Collections.Generic;
using TDGame.Systems.Enemy.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TDGame.Systems.Enemy.Systems
{
    public class EnemyMovementSystem : SystemBase
    {
        public float3[] waypoints;

        protected override void OnUpdate()
        {
            NativeArray<float3> floatsies = new NativeArray<float3>(waypoints, Allocator.TempJob);
            var deltaTime = Time.DeltaTime;
            Entities.ForEach((ref Translation translation, ref MoveForward moveForward) =>
            {
                var distna = floatsies[moveForward.waypointIndex].
                if ()
                translation.Value += moveForward.Speed * deltaTime;

            }).Schedule();

            floatsies.Dispose();
        }
    }
}