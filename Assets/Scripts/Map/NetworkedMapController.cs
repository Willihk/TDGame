using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using TDGame.Events.Base;
using TDGame.Pathfinding;
using TDGame.Pathfinding.DotsAStar;
using TDGame.Systems.Grid.InGame;
using Unity.Mathematics;
using UnityEngine;

namespace TDGame.Map
{
    public class NetworkedMapController : NetworkBehaviour
    {
        [SerializeField]
        private GameObject mapPrefab;

        [SerializeField]
        GameEvent mapLoadedEvent;

        GameObject mapObject;

        private void Start()
        {
            mapObject = Instantiate(mapPrefab);
            Old_GridController.Instance.OnMapLoaded();
            mapLoadedEvent.Raise();
        }

        public List<int2> GetWaypoints()
        {
            var grid = Old_GridController.Instance.mapGrid;
            var waypointController = mapPrefab.GetComponentInChildren<WaypointController>();
            int2 start =
                new int2(Old_GridController.Instance.mapGrid.WorldToGridPosition(waypointController.startPoint.position).x,
                    Old_GridController.Instance.mapGrid.WorldToGridPosition(waypointController.startPoint.position).y);
            int2 end = new int2(
                Old_GridController.Instance.mapGrid.WorldToGridPosition(waypointController.endPoint.position).x,
                Old_GridController.Instance.mapGrid.WorldToGridPosition(waypointController.endPoint.position).y);

            var path = new Pathfinder().FindPath(end, start, new int2(Old_GridController.Instance.gridSize),
                Old_GridController.Instance.mapGrid);
            return path;
        }
    }
}