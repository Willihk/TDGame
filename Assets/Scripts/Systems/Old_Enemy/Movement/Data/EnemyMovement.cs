using System.Collections.Generic;
using TDGame.Systems.Stats;
using UnityEngine;
using UnityEngine.Events;

namespace TDGame.Systems.Old_Enemy.Movement.Data
{
    public class EnemyMovement : MonoBehaviour
    {
        [SerializeField]
        private bool autoRegister = true;


        [SerializeField]
        protected NetworkedStatsController statsController;

        public StatWrapper speedStat;


        public List<Vector3> waypoints;
        public int currentWaypointIndex;
        public Vector3 currentWaypoint;

        public UnityEvent reachedEndEvent;

        private void Awake()
        {
            reachedEndEvent ??= new UnityEvent();
        }
        
        private void Start()
        {
            speedStat = statsController.GetStatByName(speedStatName);
        }
        
        [SerializeField]
        private string speedStatName = "Speed";

        public void Setup(List<Vector3> waypointsToReach)
        {
            waypoints = waypointsToReach;
            currentWaypoint = waypoints[0];
        }

        private void OnEnable()
        {
            if (autoRegister)
            {
                EnemyMovementSystem.Instance.RegisterEnemy(gameObject);
            }
        }

        private void OnDisable()
        {
            if (autoRegister)
            {
                EnemyMovementSystem.Instance.UnregisterEnemy(gameObject);
            }
        }
        
        public void ReachedEnd()
        {
            reachedEndEvent.Invoke();
        }
    }
}