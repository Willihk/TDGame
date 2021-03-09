using System.Collections.Generic;
using Mirror;
using TDGame.Systems.Stats;
using UnityEngine;

namespace TDGame.Systems.Enemy.Movement.Base
{
    public class BaseMovementController : NetworkBehaviour
    {
        [SerializeField]
        protected NetworkedStatsController statsController;
        
        protected List<Vector3> waypoints;

        public void Setup(List<Vector3> waypointsToReach)
        {
            waypoints = waypointsToReach;
        }
    }
}