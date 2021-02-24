using System;
using System.Collections.Generic;
using Mirror;
using TDGame.Systems.Economy;
using TDGame.Systems.Health;
using TDGame.Systems.Health.Global;
using TDGame.Systems.Health.Unit;
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

        [SerializeField]
        private float reachedEndDamage = 3;

        [SyncVar]
        private Vector3 currentWaypoint;

        [SyncVar]
        private bool hasWaypoint;

        private List<Vector3> waypoints;
        private int currentWaypointIndex;

        private void Update()
        {
            if (isServer && Vector3.Distance(transform.position, currentWaypoint) < .1f)
            {
                transform.position = currentWaypoint;
                currentWaypointIndex++;
                if (currentWaypointIndex >= waypoints.Count)
                {
                    hasWaypoint = false;
                    ReachedEnd();
                    return;
                }

                currentWaypoint = waypoints[currentWaypointIndex];
                transform.LookAt(currentWaypoint);
                Rpc_WaypointReached(transform.position);
                return;
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

        [Server]
        void ReachedEnd()
        {
            GlobalHealthSystem.Instance.ReduceHealth(reachedEndDamage);
            EnemyTargetsController.Instance.targets.Remove(gameObject);
            NetworkServer.Destroy(gameObject);
        }

        [ServerCallback]
        public void Damage(float hitDamage)
        {
            healthSystem.Damage(hitDamage);

            if (healthSystem.Health <= 0)
            {
                EnemyTargetsController.Instance.targets.Remove(gameObject);
                NetworkServer.Destroy(gameObject);
                PlayerEconomyManager.Instance.AddCurrencyToAllPlayers((int) reachedEndDamage);
            }
        }
    }
}