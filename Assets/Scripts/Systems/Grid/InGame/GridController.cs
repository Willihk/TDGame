using Mirror;
using TDGame.Map;
using TDGame.Systems.Grid.Cell.Implementations;
using TDGame.Systems.Grid.Cell.Interfaces;
using TDGame.Systems.Grid.Data;
using UnityEngine;

namespace TDGame.Systems.Grid.InGame
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

        void CreateGrid()
        {
            mapGrid = new Grid2D((int) gridSize.x, (int) gridSize.y, cellSize);
            towerGrid = new Grid2D((int) gridSize.x, (int) gridSize.y, cellSize);
        }

        void RegisterObstacles()
        {
            var obstacles = FindObjectsOfType<GridObstacle>();
            foreach (var gridObstacle in obstacles)
            {
                gridObstacle.area.position = mapGrid.WorldToGridPosition(gridObstacle.originPoint.position);
                mapGrid.SetAreaToCell(gridObstacle.area, new GameObjectCell() {Owner = gridObstacle.gameObject});
            }
        }

        public void OnMapLoaded()
        {
            var mapdetails = FindObjectOfType<MapDetailsController>();

            gridSize = mapGrid.WorldToGridPosition(mapdetails.gridTopRightCorner.transform.position);
            CreateGrid();
            RegisterObstacles();
        }

        private void Start()
        {
            InvokeRepeating(nameof(DrawGrid), 0, 1);
        }

        public bool CanPlaceTower(GameObject tower, GridArea area)
        {
            var gridPos = mapGrid.WorldToGridPosition(tower.transform.position);
            return mapGrid.IsAreaEmpty(area) && towerGrid.IsAreaEmpty(area);
        }

        [Server]
        public void PlaceTowerOnGrid(GameObject tower, GridArea area)
        {
            // TODO: check validity of placement
            var gridPos = mapGrid.WorldToGridPosition(tower.transform.position);
            towerGrid.SetAreaToCell(area, new GameObjectCell() {Owner = tower});

            Rpc_AddTowerToGrid(tower);
        }

        [ClientRpc]
        public void Rpc_AddTowerToGrid(GameObject tower)
        {
            if (isServer)
                return;

            var gridPos = mapGrid.WorldToGridPosition(tower.transform.position);
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

                    Debug.DrawLine(mapGrid.GridToWorldPosition(x, y), mapGrid.GridToWorldPosition(x, y + 1), color, 1);
                    Debug.DrawLine(mapGrid.GridToWorldPosition(x, y), mapGrid.GridToWorldPosition(x + 1, y), color, 1);
                }
            }

            Debug.DrawLine(mapGrid.GridToWorldPosition(0, (int) gridSize.y),
                mapGrid.GridToWorldPosition((int) gridSize.x, (int) gridSize.y),
                Color.white, 100f);
            Debug.DrawLine(mapGrid.GridToWorldPosition((int) gridSize.x, 0),
                mapGrid.GridToWorldPosition((int) gridSize.x, (int) gridSize.y),
                Color.white, 100f);
        }
    }
}