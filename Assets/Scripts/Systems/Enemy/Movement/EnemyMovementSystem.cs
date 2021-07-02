using System;
using System.Collections.Generic;
using TDGame.Systems.Enemy.Movement.Data;
using UnityEngine;

namespace TDGame.Systems.Enemy.Movement
{
    public class EnemyMovementSystem : MonoBehaviour
    {
        public static EnemyMovementSystem Instance;

        private List<EnemyMovement> trackedEnemies = new List<EnemyMovement>();

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            for (int i = 0; i < trackedEnemies.Count; i++)
            {
                MoveEnemy(i);
            }
        }

        void MoveEnemy(int index)
        {
            var enemy = trackedEnemies[index];
            if (Vector3.Distance(enemy.transform.position, enemy.currentWaypoint) > .1f)
            {
                enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, enemy.currentWaypoint,
                    enemy.speedStat.stat.Value * Time.deltaTime);
                return;
            }
            
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