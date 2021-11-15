using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
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
        public async void OnGridInitialized()
        {
            await UniTask.Delay(10000);

            GridManager gm = GridManager.Instance;
            WaypointController waypoint = FindObjectOfType<WaypointController>();//:O

            int2 start = gm.mapGrid.WorldToGridPosition(waypoint.startPoint.position);
            int2 end = gm.mapGrid.WorldToGridPosition(waypoint.endPoint.position);

            List<int2> path = new Pathfinder().FindPath(start, end, gm.gridSize, gm.mapGrid);


            World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EnemyMovementSystem>().path = path.Select((x) => (float3)gm.mapGrid.GridToWorldPosition(x)).ToArray();
        }
    }
}