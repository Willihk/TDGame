using System;
using System.Collections.Generic;
using Mirror;
using TDGame.HealthSystem;
using TDGame.Systems.Targeting.Data;
using UnityEngine;

namespace TDGame.Enemy.Base
{
    public class NetworkedEnemy : NetworkBehaviour
    {
        [SerializeField]
        private NetworkedHealthSystem healthSystem;

        [SerializeField]
        private float speed;

        [SyncVar]
        private Vector3 currentWaypoint;

        [SyncVar]
        private bool hasWaypoint;

        private List<Vector3> waypoints;
        private int currentWaypointIndex;

        private void Update()
        {
            if (isServer)
            {
                if (Vector3.Distance(transform.position, currentWaypoint) < .1f)
                {
                    transform.position = currentWaypoint;
                    currentWaypointIndex++;
                    if (currentWaypointIndex >= waypoints.Count)
                    {
                        hasWaypoint = false;
                        return;
                    }

                    currentWaypoint = waypoints[currentWaypointIndex];
                    transform.LookAt(currentWaypoint);
                    Rpc_WaypointReached(transform.position);
                    return;
                }
            }

            if (Vector3.Distance(transform.position, currentWaypoint) > .1f)
                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
        }

        [ClientRpc]
        void Rpc_WaypointReached(Vector3 position)
        {
            transform.position = position;
            transform.LookAt(currentWaypoint);
        }

        public void Setup(List<Vector3> waypoints)
        {
            this.waypoints = waypoints;
            currentWaypoint = waypoints[0];
            hasWaypoint = true;
        }

        [ServerCallback]
        public void Damage(float hitDamage)
        {
            healthSystem.Damage(hitDamage);
            Debug.Log("hit  " + healthSystem.Health);

            if (healthSystem.Health <= 0)
            {
                EnemyTargetsController.Instance.targets.Remove(gameObject);
                NetworkServer.Destroy(gameObject);
            }
        }
    }
}