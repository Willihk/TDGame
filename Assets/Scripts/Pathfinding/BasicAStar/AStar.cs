using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TDGame.Systems.Grid;
using TDGame.Systems.Grid.Cell;
using UnityEngine;

namespace TDGame.Pathfinding.BasicAStar
{
    public class AStar
    {
        private Grid2D grid;
        private Vector2Int start;
        private Vector2Int end;

        public List<Vector2Int> GetPath(Grid2D grid, Vector2Int start, Vector2Int end)
        {
            this.grid = grid;
            this.start = start;
            this.end = end;

            Node startNode = new Node(start);
            Node targetNode = new Node(end);

            Heap<Node> openSet = new Heap<Node>(grid.sizeX * grid.sizeY);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode.Position == end)
                {
                    return RetracePath(startNode, targetNode).Select(x => x.Position).ToList();
                }

                foreach (Node neighbour in GetNeighbours(currentNode))
                {
                    if (grid.GetCell(neighbour.Position.x, neighbour.Position.y).State != GridCellState.Path ||
                        closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.G + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.G || !openSet.Contains(neighbour))
                    {
                        neighbour.G = newMovementCostToNeighbour;
                        neighbour.H = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }

            return null;
        }

        List<Node> RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }

            path.Reverse();

            return path;
        }

        int GetDistance(Node nodeA, Node nodeB)
        {
            int dstX = Mathf.Abs(nodeA.Position.x - nodeB.Position.x);
            int dstY = Mathf.Abs(nodeA.Position.y - nodeB.Position.y);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }

        List<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    int checkX = node.Position.x + x;
                    int checkY = node.Position.y + y;

                    if (checkX >= 0 && checkX < grid.sizeX && checkY >= 0 && checkY < grid.sizeY)
                    {
                        neighbours.Add(new Node(new Vector2Int(checkX, checkY)));
                    }
                }
            }

            return neighbours;
        }

        List<Node> GetValidAdjacentNodes(Node node)
        {
            List<Node> nodes = new List<Node>();
            int x = node.Position.x;
            int y = node.Position.y;

            if (IsValidPosition(new Vector2Int(x, y - 1)))
                nodes.Add(ConvertPositionToNode(new Vector2Int(x, y - 1)));
            if (IsValidPosition(new Vector2Int(x, y + 1)))
                nodes.Add(ConvertPositionToNode(new Vector2Int(x, y + 1)));
            if (IsValidPosition(new Vector2Int(x - 1, y)))
                nodes.Add(ConvertPositionToNode(new Vector2Int(x - 1, y)));
            if (IsValidPosition(new Vector2Int(x + 1, y)))
                nodes.Add(ConvertPositionToNode(new Vector2Int(x + 1, y)));

            return nodes;

            bool IsValidPosition(Vector2Int pos)
            {
                if (pos.x > grid.sizeX || pos.y > grid.sizeY || pos.x < 0 ||
                    pos.y < 0) // outside of map
                    return false;

                if (grid.GetCell(pos.x, pos.y).State == GridCellState.Path)
                {
                    if ((pos == start || pos == end) && !pos.Equals(end) && !pos.Equals(start))
                    {
                        return false;
                    }

                    return true;
                }

                return false;
            }

            Node ConvertPositionToNode(Vector2Int pos)
            {
                return new Node(pos);
            }
        }

        private class Node : IHeapItem<Node>
        {
            public int G;
            public int H;

            public int F => H + G;

            public Vector2Int Position;
            public Node parent;

            public int HeapIndex { get; set; }

            public Node(Vector2Int position, Node parent = null)
            {
                Position = position;
                this.parent = parent;
            }

            public int CompareTo(Node nodeToCompare)
            {
                int compare = F.CompareTo(nodeToCompare.F);
                if (compare == 0)
                {
                    compare = H.CompareTo(nodeToCompare.H);
                }

                return -compare;
            }

            public override bool Equals(object obj)
            {
                if (obj is Node node)
                    return Position == node.Position;

                return false;
            }
        }
    }
}