using System;
using System.Linq;
using TDGame.Systems.Grid.Cell.Base;
using TDGame.Systems.Grid.Cell.Implementations;
using TDGame.Systems.Grid.Data;
using UnityEngine;

namespace TDGame.Systems.Grid
{
    public class Grid2D : IDisposable
    {
        public BaseCell[] grid;

        public float CellSize => cellSize;

        private readonly float cellSize = 0.5f;

        private int sizeX;
        private int sizeY;

        public Grid2D(int x, int y)
        {
            sizeX = x;
            sizeY = y;
            grid = new BaseCell[x * y];
        }

        public Grid2D(int x, int y, float cellSize) : this(x, y)
        {
            this.cellSize = cellSize;
        }


        public void SetEmpty()
        {
            for (var i = 0; i < grid.Length; i++)
            {
                grid[i] = new EmptyCell();
            }
        }

        public bool SetCell(int x, int y, BaseCell newCell)
        {
            if (!IsValidGridPosition(x, y))
                return false;

            grid[getIndex(x, y)] = newCell;

            return true;
        }

        public void SetAreaToCell(GridArea area, BaseCell newCell)
        {
            var points = area.GetPoints();

            foreach (var point in points)
            {
                SetCell(point.x, point.y, newCell);
            }
        }

        public bool SetCellIfEmpty(int x, int y, BaseCell newCell)
        {
            if (!IsCellEmpty(x, y))
                return false;

            SetCell(x, y, newCell);
            return true;
        }

        public bool IsCellEmpty(int x, int y)
        {
            if (!IsValidGridPosition(x, y))
                return false;

            var cell = grid[getIndex(x, y)];
            return cell == null || cell is EmptyCell;
        }

        public BaseCell GetCell(int x, int y)
        {
            if (!IsValidGridPosition(x, y))
                return null;

            return grid[getIndex(x, y)];
        }

        public Vector3 GridToWorldPosition(int x, int y)
        {
            return new Vector3(x, 0, y) * cellSize;
        }

        public bool IsValidGridPosition(int x, int y)
        {
            if (x < sizeX && y < sizeY && x > 0 && y > 0)
            {
                return true;
            }

            return false;
        }

        public bool IsAreaValid(GridArea area)
        {
            var points = area.GetPoints();

            for (int i = 0; i < points.Length; i++)
            {
                if (!IsValidGridPosition(points[i].x, points[i].y))
                    return false;
            }

            return true;
        }

        public bool IsAreaEmpty(GridArea area)
        {
            return area.GetPoints().All(p => IsCellEmpty(p.x, p.y));
        }

        public Vector2Int WorldToGridPosition(Vector3 worldPosition)
        {
            int x = Mathf.FloorToInt(worldPosition.x / cellSize);
            int y = Mathf.FloorToInt(worldPosition.z / cellSize);
            return new Vector2Int(x, y);
        }

        public int getIndex(int x, int y)
        {
            return y * sizeX + x;
        }

        public void Dispose()
        {
            grid = null;
        }
    }
}