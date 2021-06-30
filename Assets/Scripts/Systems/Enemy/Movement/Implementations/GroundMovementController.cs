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

        [SerializeField]
        private string speedStatName = "Speed";

        private Vector3 currentWaypoint;

        private int currentWaypointIndex;

        public UnityEvent reachedEndEvent;

        private void Awake()
        {
            reachedEndEvent ??= new UnityEvent();
        }

        private void Start()
        {
            speedStat = statsController.GetStatByName(speedStatName);
        }

        public override void OnStartServer()
        {
            currentWaypoint = waypoints[0];
        }

        private void Update()
        {
            if (!isServer)
                return;

            if (Vector3.Distance(transform.position, currentWaypoint) > .1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint,
                    speedStat.stat.Value * Time.deltaTime);
                return;
            }
            
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Count)
            {
                ReachedEnd();
                return;
            }

            currentWaypoint = waypoints[currentWaypointIndex];
            transform.LookAt(currentWaypoint);
        }

        private void ReachedEnd()
        {
            reachedEndEvent.Invoke();
        }
    }
}