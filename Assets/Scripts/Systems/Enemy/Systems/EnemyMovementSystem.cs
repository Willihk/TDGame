using System.Collections.Generic;
using TDGame.Systems.Enemy.Components;
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
            var deltaTime = Time.DeltaTime;
            Entities.ForEach((ref Translation translation,in MoveForward moveForward) =>
            {
                translation.Value += moveForward.Speed * deltaTime;
            }).Schedule();
        }
    }
}