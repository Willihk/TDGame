using System.Collections.Generic;
using Mirror;
using TDGame.Events.Base;
using UnityEngine;

namespace TDGame.Map
{
    public class NetworkedMapController : NetworkBehaviour
    {
        public bool hasLoadedMap;

        [SerializeField]
        private GameObject mapPrefab;

        [SerializeField]
        GameEvent mapLoadedEvent;

        GameObject mapObject;

        public override void OnStartClient()
        {
            mapObject = Instantiate(mapPrefab);
            mapLoadedEvent.Raise();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            mapLoadedEvent.Raise();
        }

        public IEnumerable<Transform> GetWaypoints()
        {
            if (isServer)
                return mapPrefab.GetComponentInChildren<WaypointController>().waypoints;
            else if (isClient)
                return mapObject.GetComponentInChildren<WaypointController>().waypoints;

            return mapPrefab.GetComponentInChildren<WaypointController>().waypoints;
        }
    }
}