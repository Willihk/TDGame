using System.Collections.Generic;
using System.Linq;
using TDGame.Systems.Grid;
using TDGame.Systems.Grid.Cell;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace TDGame.Pathfinding.DotsAStar
{
    public class PathFinding
    {
        public const int MOVE_STRAIGHT_COST = 10;
        public const int MOVE_DIAGONAL_COST = 14;

        public List<int2> FindPath(int2 start, int2 end, int2 gridSize, Grid2D grid)
        {
            float startTime = Time.realtimeSinceStartup;
            List<int2> pathList = new List<int2>();
            NativeArray<PathNode> pathNodes = new NativeArray<PathNode>(gridSize.x * gridSize.y, Allocator.Temp);

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    PathNode pathNode = new PathNode();
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

            NativeArray<int2> neighbourOffsets = new NativeArray<int2>(8, Allocator.Temp);
            neighbourOffsets[0] = new int2(-1, 0); // Left
            neighbourOffsets[1] = new int2(+1, 0); // Right
            neighbourOffsets[2] = new int2(0, +1); // Up
            neighbourOffsets[3] = new int2(0, -1); // Down
            neighbourOffsets[4] = new int2(-1, -1); // Left Down
            neighbourOffsets[5] = new int2(-1, +1); // Left Up
            neighbourOffsets[6] = new int2(+1, -1); // Right Down
            neighbourOffsets[7] = new int2(+1, +1); // Right Up

            int endNodeIndex = GetIndex(end.x, end.y, gridSize.x);

            var startNode = pathNodes[GetIndex(start.x, start.y, gridSize.x)];
            startNode.gCost = 0;
            startNode.CalculateFCost();
            pathNodes[startNode.index] = startNode;

            NativeList<int> openList = new NativeList<int>(Allocator.Temp);
            NativeList<int> closedList = new NativeList<int>(Allocator.Temp);

            openList.Add(startNode.index);
            
            
            Debug.Log($"Initialize Time: {(Time.realtimeSinceStartup - startTime) * 1000} ms");

            while (openList.Length > 0)
            {
                int currentNodeIndex = GetLowestFCostNodeIndex(openList, pathNodes);
                PathNode currentNode = pathNodes[currentNodeIndex];

                if (currentNodeIndex == endNodeIndex)
                {
                    break;
                }

                for (int i = 0; i < openList.Length; i++)
                {
                    if (openList[i] == currentNodeIndex)
                    {
                        openList.RemoveAtSwapBack(i);
                        break;
                    }
                }

                closedList.Add(currentNodeIndex);

                for (int i = 0; i < neighbourOffsets.Length; i++)
                {
                    int2 neighbourOffset = neighbourOffsets[i];
                    int2 neighbourPosition =
                        new int2(currentNode.x + neighbourOffset.x, currentNode.y + neighbourOffset.y);

                    if (!IsPositionInsideGrid(neighbourPosition, gridSize))
                    {
                        continue;
                    }

                    int neighbourIndex = GetIndex(neighbourPosition.x, neighbourPosition.y, gridSize.x);

                    if (closedList.Contains(neighbourIndex))
                    {
                        continue;
                    }

                    PathNode neighbourNode = pathNodes[neighbourIndex];
                    if (!neighbourNode.isWalkable)
                    {
                        // Not walkable
                        continue;
                    }

                    int2 currentNodePosition = new int2(currentNode.x, currentNode.y);

                    int tentativeGCost =
                        currentNode.gCost + CalculateDistanceCost(currentNodePosition, neighbourPosition);
                    if (tentativeGCost < neighbourNode.gCost)
                    {
                        neighbourNode.cameFromNodeIndex = currentNodeIndex;
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.CalculateFCost();
                        pathNodes[neighbourIndex] = neighbourNode;

                        if (!openList.Contains(neighbourNode.index))
                        {
                            openList.Add(neighbourNode.index);
                        }
                    }
                }
            }

            PathNode endNode = pathNodes[endNodeIndex];
            if (endNode.cameFromNodeIndex == -1)
            {
                // Didn't find a path!
                //Debug.Log("Didn't find a path!");
            }
            else
            {
                // Found a path
                var path = CalculatePath(pathNodes, endNode);

                for (int index = 0; index < path.Length; index++)
                {
                    int2 point = path[index];
                    pathList.Add(point);
                }

                path.Dispose();
            }

            openList.Dispose();
            closedList.Dispose();
            neighbourOffsets.Dispose();

            Debug.Log($"Time: {(Time.realtimeSinceStartup - startTime) * 1000} ms");
            return pathList;
        }

        private NativeList<int2> CalculatePath(NativeArray<PathNode> pathNodes, PathNode endNode)
        {
            NativeList<int2> path = new NativeList<int2>(Allocator.Temp);

            if (endNode.cameFromNodeIndex != -1)
            {
                // Found a path

                PathNode currentNode = endNode;
                while (currentNode.cameFromNodeIndex != -1)
                {
                    PathNode cameFromNode = pathNodes[currentNode.cameFromNodeIndex];
                    path.Add(new int2(cameFromNode.x, cameFromNode.y));
                    currentNode = cameFromNode;
                }
            }

            return path;
        }

        private static bool IsPositionInsideGrid(int2 gridPosition, int2 gridSize)
        {
            return
                gridPosition.x >= 0 &&
                gridPosition.y >= 0 &&
                gridPosition.x < gridSize.x &&
                gridPosition.y < gridSize.y;
        }

        int GetLowestFCostNodeIndex(NativeList<int> openList, NativeArray<PathNode> pathNodes)
        {
            var lowestCostPathNode = pathNodes[openList[0]];

            for (int i = 0; i < openList.Length; i++)
            {
                var pathNode = pathNodes[openList[i]];

                if (pathNode.fCost < lowestCostPathNode.fCost)
                {
                    lowestCostPathNode = pathNode;
                }
            }

            return lowestCostPathNode.index;
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

        struct PathNode
        {
            public bool isWalkable;

            public int x;
            public int y;

            public int index;

            public int cameFromNodeIndex;

            public int gCost;
            public int hCost;
            public int fCost;

            public void CalculateFCost()
            {
                fCost = gCost + hCost;
            }
        }
    }
}