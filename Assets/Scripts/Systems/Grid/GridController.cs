using System;
using Mirror;
using TDGame.Systems.Grid.Cell.Implementations;
using TDGame.Systems.Grid.Cell.Interfaces;
using UnityEngine;

namespace TDGame.Systems.Grid
{
    public class GridController : NetworkBehaviour
    {
        public static GridController Instance;

        public Vector2 gridSize;

        public float cellSize = .5f;

        public Grid2D mapGrid;
        public Grid2D towerGrid;

        public void Awake()
        {
            Instance = this;
            mapGrid = new Grid2D((int) gridSize.x, (int) gridSize.y, cellSize);
            towerGrid = new Grid2D((int) gridSize.x, (int) gridSize.y, cellSize);
        }

        private void Start()
        {
            InvokeRepeating(nameof(DrawGrid), 0, 1);
        }

        public bool CanPlaceTower(GameObject tower)
        {
            var gridPos = mapGrid.ConvertToGridPosition(tower.transform.position);
            return mapGrid.IsCellEmpty(gridPos.x, gridPos.y) && towerGrid.IsCellEmpty(gridPos.x, gridPos.y);
        }

        [Server]
        public void PlaceTowerOnGrid(GameObject tower)
        {
            // TODO: check validity of placement
            var gridPos = mapGrid.ConvertToGridPosition(tower.transform.position);
            towerGrid.SetCell(gridPos.x, gridPos.y, new GameObjectCell() {Owner = tower});

            Rpc_AddTowerToGrid(tower);
        }

        [ClientRpc]
        public void Rpc_AddTowerToGrid(GameObject tower)
        {
            if (isServer)
                return;

            var gridPos = mapGrid.ConvertToGridPosition(tower.transform.position);
            towerGrid.SetCell(gridPos.x, gridPos.y, new GameObjectCell() {Owner = tower});
        }

        void DrawGrid()
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    var cell = towerGrid.GetCell(x, y);
                    if (cell is IDisplayCell)
                    {
                        Debug.Log($"{x}:{y} is displayed as {cell}");
                    }

                    var color = Color.white;
                    if (cell is GameObjectCell)
                        color = Color.red;

                    Debug.DrawLine(mapGrid.GetWorldPosition(x, y), mapGrid.GetWorldPosition(x, y + 1), color, 1);
                    Debug.DrawLine(mapGrid.GetWorldPosition(x, y), mapGrid.GetWorldPosition(x + 1, y), color, 1);
                }
            }

            Debug.DrawLine(mapGrid.GetWorldPosition(0, (int) gridSize.y),
                mapGrid.GetWorldPosition((int) gridSize.x, (int) gridSize.y),
                Color.white, 100f);
            Debug.DrawLine(mapGrid.GetWorldPosition((int) gridSize.x, 0),
                mapGrid.GetWorldPosition((int) gridSize.x, (int) gridSize.y),
                Color.white, 100f);
        }
    }
}