using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using TDGame.Events.Base;
using TDGame.Pathfinding.BasicAStar;
using TDGame.Systems.Grid.InGame;
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

        private void Start()
        {
            mapObject = Instantiate(mapPrefab);
            GridController.Instance.OnMapLoaded();
            mapLoadedEvent.Raise();
        }

        public List<Vector2Int> GetWaypoints()
        {
            var grid = GridController.Instance.mapGrid;
            var waypointController = mapPrefab.GetComponentInChildren<WaypointController>();
            // var path = new AStar().GetPath(GridController.Instance.mapGrid,
            //     GridController.Instance.mapGrid.WorldToGridPosition(waypointController.startPoint.position),
            //     GridController.Instance.mapGrid.WorldToGridPosition(waypointController.endPoint.position));
            return new List<Vector2Int>()
            {
                GridController.Instance.mapGrid.WorldToGridPosition(waypointController.startPoint.position),
                GridController.Instance.mapGrid.WorldToGridPosition(waypointController.endPoint.position)
            };
            return new AStar().GetPath(GridController.Instance.mapGrid,
                GridController.Instance.mapGrid.WorldToGridPosition(waypointController.startPoint.position),
                GridController.Instance.mapGrid.WorldToGridPosition(waypointController.endPoint.position));
        }
    }
}