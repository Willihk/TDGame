using System;
using Mirror;
using TDGame.Map;
using TDGame.Settings;
using TDGame.Systems.Grid.Cell;
using TDGame.Systems.Grid.Data;
using Unity.Mathematics;
using UnityEngine;

namespace TDGame.Systems.Grid.InGame
{
    [Obsolete]
    public class Old_GridController : NetworkBehaviour
    {
        public static Old_GridController Instance;

        public Texture2D gridTexture;

        public int2 gridSize;

        public float cellSize = .5f;

        public Grid2D mapGrid;
        public Grid2D towerGrid;

        [SerializeField]
        private Material gridMaterial;

        [SerializeField]
        private LobbySettings lobbySettings;


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

        void CreateGrid()
        {
            mapGrid = new Grid2D((int) gridSize.x, (int) gridSize.y, cellSize);
            towerGrid = new Grid2D((int) gridSize.x, (int) gridSize.y, cellSize);

            gridTexture = new Texture2D(gridSize.x, gridSize.y);
            gridTexture.filterMode = FilterMode.Point;
        }

        void RegisterObstacles()
        {
            var obstacles = FindObjectsOfType<GridObstacle>();
            foreach (var gridObstacle in obstacles)
            {
                gridObstacle.area.position = mapGrid.WorldToGridPosition(gridObstacle.originPoint.position);
                mapGrid.SetAreaToCell(gridObstacle.area, new GridCell() {State = GridCellState.Occupied});
            }
        }

        void RegisterPath()
        {
            var paths = FindObjectsOfType<GridPath>();
            foreach (var gridPath in paths)
            {
                gridPath.area.position = mapGrid.WorldToGridPosition(gridPath.originPoint.position);
                mapGrid.SetAreaToCell(gridPath.area, new GridCell() {State = GridCellState.Path});
            }
        }

        public void OnMapLoaded()
        {
            Vector3 position = new Vector3(lobbySettings.selectedMap.size.x, 0, lobbySettings.selectedMap.size.y);

            gridSize = mapGrid.WorldToGridPosition(position);
            CreateGrid();
            RegisterObstacles();
            RegisterPath();
        }

        void UpdateTexture()
        {
            var pixels = gridTexture.GetPixels();

            for (int i = 0; i < towerGrid.grid.Length; i++)
            {
                pixels[i] = new Color(0, 0, 0, 0);

                if (towerGrid.grid[i].State != GridCellState.Empty)
                {
                    pixels[i] = Color.white;
                }

                if (mapGrid.grid[i].State != GridCellState.Empty)
                {
                    pixels[i] = Color.magenta;
                }
            }

            gridTexture.SetPixels(pixels);
            gridTexture.Apply();
            gridMaterial.SetTexture("GridTexture", gridTexture);
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
            towerGrid.SetAreaToCell(area, new GridCell() {State = GridCellState.Occupied});

            Rpc_AddTowerToGrid(tower);
        }

        [Server]
        public void EmptyGridArea(GridType gridType, GridArea gridArea)
        {
            switch (gridType)
            {
                case GridType.Map:
                    mapGrid.SetAreaCellState(gridArea, GridCellState.Empty);
                    break;
                case GridType.Tower:
                    towerGrid.SetAreaCellState(gridArea, GridCellState.Empty);
                    break;
            }

            Rpc_EmptyGridArea(gridType, gridArea);
        }

        [ClientRpc]
        void Rpc_EmptyGridArea(GridType gridType, GridArea gridArea)
        {
            if (isServer) // Host does not need to run this
                return;

            switch (gridType)
            {
                case GridType.Map:
                    mapGrid.SetAreaCellState(gridArea, GridCellState.Empty);
                    break;
                case GridType.Tower:
                    towerGrid.SetAreaCellState(gridArea, GridCellState.Empty);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gridType), gridType, "Invalid grid type");
            }
        }

        [ClientRpc]
        public void Rpc_AddTowerToGrid(GameObject tower)
        {
            if (isServer) // Host does not need to run this
                return;

            var gridPos = mapGrid.WorldToGridPosition(tower.transform.position);
            towerGrid.SetCell(gridPos.x, gridPos.y, new GridCell() {State = GridCellState.Occupied});
        }

        void DrawGrid()
        {
            UpdateTexture();
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    var cell = towerGrid.GetCell(x, y);
                    var mapCell = mapGrid.GetCell(x, y);

                    Color color = Color.white;
                    if (cell.State == GridCellState.Empty && mapCell.State == GridCellState.Empty)
                        color = Color.white;
                    else if (cell.State == GridCellState.Occupied)
                        color = Color.red;
                    else if (mapCell.State == GridCellState.Path)
                        color = Color.yellow;

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