using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using TDGame.Events.Base;
using TDGame.Pathfinding.BasicAStar;
using TDGame.Pathfinding.DotsAStar;
using TDGame.Systems.Grid.InGame;
using Unity.Mathematics;
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

        public List<int2> GetWaypoints()
        {
            var grid = GridController.Instance.mapGrid;
            var waypointController = mapPrefab.GetComponentInChildren<WaypointController>();
            int2 start =
                new int2(GridController.Instance.mapGrid.WorldToGridPosition(waypointController.startPoint.position).x,
                    GridController.Instance.mapGrid.WorldToGridPosition(waypointController.startPoint.position).y); 
            int2 end = new int2(
                GridController.Instance.mapGrid.WorldToGridPosition(waypointController.endPoint.position).x,
                GridController.Instance.mapGrid.WorldToGridPosition(waypointController.endPoint.position).y);
            
            var path = new PathFinding().FindPath(end, start, new int2(GridController.Instance.gridSize), GridController.Instance.mapGrid);
            return path;
        }
    }
}