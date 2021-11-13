using System;
using System.Collections.Generic;
using TDGame.Systems.Old_Enemy.Movement.Data;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

namespace TDGame.Systems.Old_Enemy.Movement
{
    public class EnemyMovementSystem : MonoBehaviour
    {
        public static EnemyMovementSystem Instance;

        private List<EnemyMovement> trackedEnemies = new List<EnemyMovement>();

        private NativeQueue<EnemyMovementEvent> movementEvents;

        private JobHandle handle;
        private TransformAccessArray transformArray;

        private int jobCount;

        private void Awake()
        {
            Instance = this;
            movementEvents = new NativeQueue<EnemyMovementEvent>(Allocator.Persistent);

            jobCount = Environment.ProcessorCount - 1;
        }

        private void OnDestroy()
        {
            movementEvents.Dispose();
        }

        private void Update()
        {
            MoveEnemiesUsingJob();
        }

        void MoveEnemiesUsingJob()
        {
            var transforms = new Transform[trackedEnemies.Count];

            var movementDatas = new NativeArray<EnemyMovementData>(trackedEnemies.Count, Allocator.TempJob);

            // Get all needed data for moving an enemy
            for (int i = 0; i < trackedEnemies.Count; i++)
            {
                transforms[i] = trackedEnemies[i].transform;

                movementDatas[i] = new EnemyMovementData()
                {
                    currentWaypoint = trackedEnemies[i].currentWaypoint,
                    speed = trackedEnemies[i].speedStat.stat.Value
                };
            }

            transformArray = new TransformAccessArray(transforms, jobCount);

            handle = new EnemyMovementJob()
            {
                DeltaTime = Time.deltaTime,
                MovementDatas = movementDatas,
                MovementEvents = movementEvents.AsParallelWriter()
            }.Schedule(transformArray);
        }

        private void LateUpdate()
        {
            handle.Complete();
            transformArray.Dispose();
            
            while (movementEvents.TryDequeue(out EnemyMovementEvent movementEvent))
            {
                if (movementEvent.EventType != MovementEvent.ReachedWaypoint)
                    continue;

                var enemy = trackedEnemies[movementEvent.Index];
                enemy.currentWaypointIndex++;
                if (enemy.currentWaypointIndex >= enemy.waypoints.Count)
                {
                    // reached end
                    enemy.ReachedEnd();
                    return;
                }

                enemy.currentWaypoint = enemy.waypoints[enemy.currentWaypointIndex];
                enemy.transform.LookAt(enemy.currentWaypoint);
            }
        }


        public bool RegisterEnemy(GameObject enemyObject)
        {
            var enemy = enemyObject.GetComponent<EnemyMovement>();
            if (trackedEnemies.Contains(enemy))
                return false;

            trackedEnemies.Add(enemy);

            return true;
        }

        public bool UnregisterEnemy(GameObject enemyObject)
        {
            var enemy = enemyObject.GetComponent<EnemyMovement>();
            if (!trackedEnemies.Contains(enemy))
                return false;

            trackedEnemies.Remove(enemy);

            return true;
        }
    }
}