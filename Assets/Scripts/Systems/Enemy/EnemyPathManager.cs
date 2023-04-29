using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TDGame.Events;
using TDGame.Map;
using TDGame.Pathfinding;
using TDGame.Systems.Enemy.Systems;
using TDGame.Systems.Enemy.Systems.Movement;
using TDGame.Systems.Grid.InGame;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


namespace TDGame.Systems.Enemy
{
    public class EnemyPathManager : MonoBehaviour
    {
        private void Start()
        {
            EventManager.Instance.onGridInitialized.EventListeners += OnGridInitialized;
        }

        private void OnDestroy()
        {
            EventManager.Instance.onGridInitialized.EventListeners -= OnGridInitialized;
        }

        public void OnGridInitialized()
        {
            UniTask.Void(async () =>
            {
                await UniTask.Delay(1000);

                GridManager gridManager = GridManager.Instance;
                WaypointController waypoint = FindObjectOfType<WaypointController>(); //:O

                int2 start = gridManager.mapGrid.WorldToGridPosition(waypoint.startPoint.position);
                int2 end = gridManager.mapGrid.WorldToGridPosition(waypoint.endPoint.position);

                List<int2> path = new Pathfinder().FindPath(start, end, gridManager.gridSize, gridManager.mapGrid);


                World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<EnemyMovementSystem>().path =
                    path.Select((x) => (float3)gridManager.mapGrid.GridToWorldPosition(x)).ToArray();

                EventManager.Instance.onPathRegistered.Raise();
            });
        }
    }
}