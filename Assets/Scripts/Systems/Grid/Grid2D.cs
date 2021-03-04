using System;
using TDGame.Systems.Grid.Cell.Base;
using TDGame.Systems.Grid.Cell.Implementations;
using UnityEngine;

namespace TDGame.Systems.Grid
{
    public class Grid2D : IDisposable
    {
        public BaseCell[] grid;

        private float cellSize = 0.5f;

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

        public void SetCell(int x, int y, BaseCell value)
        {
            if (!IsValidPoint(x, y))
                return;
            
            grid[getIndex(x, y)] = value;
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
            if (!IsValidPoint(x, y))
                return false;

            var cell = grid[getIndex(x, y)];
            return cell == null || cell is EmptyCell;
        }

        public BaseCell GetCell(int x, int y)
        {
            if (!IsValidPoint(x, y))
                return null;
            
            return grid[getIndex(x, y)];
        }

        public Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(x, 0, y) * cellSize;
        }

        bool IsValidPoint(int x, int y)
        {
            if (x < sizeX && y < sizeY && x > 0 && y > 0)
            {
                return true;
            }
            Debug.Log($"{x}-{y} is invalid");
            return false;
        }

        public Vector2Int ConvertToGridPosition(Vector3 worldPosition)
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