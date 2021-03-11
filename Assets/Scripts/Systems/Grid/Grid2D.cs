using System;
using System.Linq;
using TDGame.Systems.Grid.Cell;
using TDGame.Systems.Grid.Data;
using UnityEngine;

namespace TDGame.Systems.Grid
{
    public class Grid2D : IDisposable
    {
        public GridCell[] grid;

        public float CellSize => cellSize;

        private readonly float cellSize = 0.5f;

        private int sizeX;
        private int sizeY;

        public Grid2D(int x, int y)
        {
            sizeX = x;
            sizeY = y;
            grid = new GridCell[x * y];
        }

        public Grid2D(int x, int y, float cellSize) : this(x, y)
        {
            this.cellSize = cellSize;
        }

        public void ClearGrid()
        {
            for (var i = 0; i < grid.Length; i++)
            {
                grid[i] = new GridCell();
            }
        }

        #region CellModifiers

        public bool SetCellState(int x, int y, GridCellState newState)
        {
            if (!IsValidGridPosition(x, y))
                return false;
            grid[getIndex(x, y)].State = newState;
            return true;
        }

        public bool SetAreaCellState(GridArea area, GridCellState newState)
        {
            var points = area.GetPoints();

            foreach (var point in points)
            {
                SetCellState(point.x, point.y, newState);
            }

            return true;
        }

        #endregion

        public bool SetCell(int x, int y, GridCell newCell)
        {
            if (!IsValidGridPosition(x, y))
                return false;

            grid[getIndex(x, y)] = newCell;

            return true;
        }

        public void SetAreaToCell(GridArea area, GridCell newCell)
        {
            var points = area.GetPoints();

            foreach (var point in points)
            {
                SetCell(point.x, point.y, newCell);
            }
        }

        public bool IsCellEmpty(int x, int y)
        {
            if (!IsValidGridPosition(x, y))
                return false;

            var cell = grid[getIndex(x, y)];
            return cell.State == GridCellState.Empty;
        }

        public GridCell GetCell(int x, int y)
        {
            if (!IsValidGridPosition(x, y))
                throw new ArgumentException("Invalid grid position");

            return grid[getIndex(x, y)];
        }

        public Vector3 GridToWorldPosition(int x, int y)
        {
            return new Vector3(x, 0, y) * cellSize;
        }

        public bool IsValidGridPosition(int x, int y)
        {
            if (x < sizeX && y < sizeY && x >= 0 && y >= 0)
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