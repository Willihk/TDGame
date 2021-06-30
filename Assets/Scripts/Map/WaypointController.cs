using System;
using System.Collections.Generic;
using UnityEngine;

namespace TDGame.Map
{
    public class WaypointController : MonoBehaviour
    {
        public Transform startPoint;
        public Transform endPoint;
        [SerializeField]
        public List<Transform> waypoints;

        private void Awake()
        {
            if (waypoints != null && waypoints.Count != 0)
                return;

            waypoints = new List<Transform>();
            foreach (Transform child in transform)
            {
                waypoints.Add(child);
            }
        }
    }
}