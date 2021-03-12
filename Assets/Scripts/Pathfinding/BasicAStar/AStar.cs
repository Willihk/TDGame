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
            List<Vector2Int> path = new List<Vector2Int>();

            Node startNode = new Node(start);
            Node endNode = new Node(end);

            Heap<Node> openList = new Heap<Node>(grid.grid.Length);
            HashSet<Node> closedList = new HashSet<Node>();

            openList.Add(startNode);
            Node currentNode = null;

            while (openList.Count > 0 && closedList.All(x => x.Position != end)) // Loop til end is found
            {
                currentNode = openList.RemoveFirst();

                closedList.Add(currentNode);

                List<Node> children = GetValidAdjacentNodes(currentNode);

                foreach (Node child in children)
                {
                    if (closedList.Contains(child) || openList.Contains(child))
                        continue;

                    child.parent = currentNode;
                    child.H = Math.Abs(child.Position.x - endNode.Position.x) +
                              Math.Abs(child.Position.y - endNode.Position.y);
                    child.G = currentNode.G + 1;

                    openList.Add(child);
                }
            }

            if (currentNode.Equals(endNode)) // Found goal
            {
                Node current = currentNode;

                while (current != null)
                {
                    path.Add(current.Position);
                    current = current.parent;
                }
            }

            return path;
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

                return grid.GetCell(pos.x, pos.y).State == GridCellState.Path;
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

            public int F
            {
                get { return H + G; }
            }

            public Vector2Int Position;
            public Node parent;

            public int HeapIndex { get; set; }

            public Node(Vector2Int position, Node parent = null)
            {
                Position = position;
                this.parent = parent;
            }

            public int CompareTo(Node other)
            {
                int compare = F.CompareTo(other.F);
                if (compare == 0)
                    compare = H.CompareTo(other.H);

                return -compare;
            }

            public override bool Equals(object obj)
            {
                if (obj is Node node)
                    return Position.Equals(node.Position);

                return false;
            }
        }
    }
}