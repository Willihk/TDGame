using System.Collections.Generic;
using TDGame.Systems.Grid;
using TDGame.Systems.Grid.Cell;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace TDGame.Pathfinding.DotsAStar
{
    public struct PathNode
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
    
    [BurstCompile]
    public struct AStarJob : IJob
    {
        public const int MOVE_STRAIGHT_COST = 10;
        public const int MOVE_DIAGONAL_COST = 14;
        
        public NativeArray<PathNode> pathNodes;
        public NativeList<int2> finalPath;

        public int2 start;
        public int2 end;
        public int2 gridSize;
        
        public void Execute()
        {
            FindPath();
        }

        private void FindPath()
        {
            var neighbourOffsets = new NativeArray<int2>(8, Allocator.Temp);
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
                    finalPath.Add(point);
                }

                path.Dispose();
            }

            openList.Dispose();
            closedList.Dispose();
            neighbourOffsets.Dispose();
        }

        private static NativeList<int2> CalculatePath(NativeArray<PathNode> pathNodes, PathNode endNode)
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

        private static int GetLowestFCostNodeIndex(NativeList<int> openList, NativeArray<PathNode> pathNodes)
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

        private static int GetIndex(int x, int y, int width)
        {
            return y * width + x;
        }

        private static int CalculateDistanceCost(int2 positionA, int2 positionB)
        {
            int distanceX = math.abs(positionA.x - positionB.x);
            int distanceY = math.abs(positionA.y - positionB.y);
            int remaining = math.abs(distanceX - distanceY);
            return MOVE_DIAGONAL_COST * math.min(distanceX, distanceY) + MOVE_STRAIGHT_COST * remaining;
        }
    }
}