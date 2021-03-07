using System;
using Mirror;
using TDGame.Systems.Enemy.Movement.Base;
using TDGame.Systems.Stats;
using UnityEngine;
using UnityEngine.Events;

namespace TDGame.Systems.Enemy.Movement.Implementations
{
    public class GroundMovementController : BaseMovementController
    {
        private StatWrapper speedStat;

        [SyncVar]
        private Vector3 currentWaypoint;

        private int currentWaypointIndex;

        public UnityEvent reachedEndEvent;

        private void Awake()
        {
            reachedEndEvent ??= new UnityEvent();
        }

        private void Start()
        {
            speedStat = statsController.GetStatByName("Speed");
            currentWaypoint = waypoints[0];
        }

        private void Update()
        {
            if (Vector3.Distance(transform.position, currentWaypoint) > .1f)
                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint,
                    speedStat.stat.Value * Time.deltaTime);

            if (!isServer || !(Vector3.Distance(transform.position, currentWaypoint) < .1f))
                return;

            transform.position = currentWaypoint;
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Count)
            {
                ReachedEnd();
                return;
            }

            currentWaypoint = waypoints[currentWaypointIndex];
            transform.LookAt(currentWaypoint);
            Rpc_WaypointReached(transform.position);
        }

        [ClientRpc]
        void Rpc_WaypointReached(Vector3 position)
        {
            transform.position = position;
            transform.LookAt(currentWaypoint);
        }

        private void ReachedEnd()
        {
            reachedEndEvent.Invoke();
        }
    }
}