using System.Collections.Generic;
using System.Linq;
using TDGame.Systems.Grid;
using TDGame.Systems.Grid.Cell;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace TDGame.Pathfinding.DotsAStar
{
    public class AStar
    {
        public const int MOVE_STRAIGHT_COST = 10;
        public const int MOVE_DIAGONAL_COST = 14;

        public List<int2> FindPath(int2 start, int2 end, int2 gridSize, Grid2D grid)
        {
            float startTime = Time.realtimeSinceStartup;
            var pathList = new NativeList<int2>(Allocator.TempJob);
            
            var pathNodes = new NativeArray<PathNode>(gridSize.x * gridSize.y, Allocator.TempJob);

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    var pathNode = new PathNode();
                    pathNode.x = x;
                    pathNode.y = y;

                    pathNode.index = GetIndex(x, y, gridSize.x);

                    pathNode.gCost = int.MaxValue;
                    pathNode.hCost = CalculateDistanceCost(new int2(x, y), end);
                    pathNode.CalculateFCost();

                    pathNode.isWalkable = grid.GetCell(x,y).State == GridCellState.Path;

                    pathNode.cameFromNodeIndex = -1;

                    pathNodes[pathNode.index] = pathNode;
                }
            }



            var jobStart = Time.realtimeSinceStartup - startTime;
            Debug.Log($"Initialize Time: {(Time.realtimeSinceStartup - startTime) * 1000} ms");

            var job = new AStarJob()
            {
                start = start,
                end = end,
                finalPath = pathList,
                gridSize = gridSize,
                pathNodes = pathNodes
            };
            job.Run();

            Debug.Log($"Pathing Time: {(Time.realtimeSinceStartup - jobStart) * 1000} ms");

            var path = pathList.AsArray().AsSpan().ToArray().ToList();

            pathList.Dispose();
            pathNodes.Dispose();
            return path;
        }

        public int GetIndex(int x, int y, int width)
        {
            return y * width + x;
        }

        public int CalculateDistanceCost(int2 positionA, int2 positionB)
        {
            int distanceX = math.abs(positionA.x - positionB.x);
            int distanceY = math.abs(positionA.y - positionB.y);
            int remaining = math.abs(distanceX - distanceY);
            return MOVE_DIAGONAL_COST * math.min(distanceX, distanceY) + MOVE_STRAIGHT_COST * remaining;
        }
    }
}