using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

namespace TDGame.Systems.Enemy.Movement
{
    [BurstCompatible]
    public struct EnemyMovementJob : IJobParallelForTransform
    {
        public float DeltaTime;

        [DeallocateOnJobCompletion]
        public NativeArray<EnemyMovementData> MovementDatas;

        [WriteOnly]
        public NativeQueue<EnemyMovementEvent>.ParallelWriter MovementEvents;

        public void Execute(int index, TransformAccess transform)
        {
            var data = MovementDatas[index];
            if (math.distance(transform.position, data.currentWaypoint) > .1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, data.currentWaypoint,
                    data.speed * DeltaTime);
                return;
            }

            MovementEvents.Enqueue(new EnemyMovementEvent()
                { Index = index, EventType = MovementEvent.ReachedWaypoint });
        }
    }

    [Serializable]
    public struct EnemyMovementData
    {
        public float3 currentWaypoint;
        public float speed;
    }
    
    [Serializable]
    public struct EnemyMovementEvent
    {
        public int Index;
        public MovementEvent EventType;
    }
    
    [Serializable]
    public enum MovementEvent
    {
        ReachedWaypoint
    }
}