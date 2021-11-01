using System;
using System.IO;
using MessagePack;
using Mirror;
using TDGame.Network.Components.Messaging;
using TDGame.Settings;
using TDGame.Systems.Grid.Cell;
using TDGame.Systems.Grid.Data;
using TDGame.Systems.Grid.Messages.Server;
using Unity.Mathematics;
using UnityEngine;
using NetworkConnection = TDGame.Network.Components.Messaging.NetworkConnection;

namespace TDGame.Systems.Grid.InGame
{
    public class GridManager : MonoBehaviour
    {
         public static GridManager Instance;

        public Texture2D gridTexture;

        public int2 gridSize;

        public float cellSize = .5f;

        public Grid2D mapGrid;
        public Grid2D towerGrid;

        [SerializeField]
        private Material gridMaterial;

        [SerializeField]
        private LobbySettings lobbySettings;

        private BaseMessagingManager messagingManager;

        public void Awake()
        {
            Instance = this;
            mapGrid = new Grid2D((int) gridSize.x, (int) gridSize.y, cellSize);
            towerGrid = new Grid2D((int) gridSize.x, (int) gridSize.y, cellSize);
        }
        
        private void Start()
        {
            InvokeRepeating(nameof(DrawGrid), 0, 1);
            
            messagingManager = BaseMessagingManager.Instance;
            
            messagingManager.RegisterNamedMessageHandler<SetGridAreaMessage>(Handle_SetGridArea);
            
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


        #region Server

        public void PlaceTowerOnGrid(GameObject tower, Vector3 position, GridArea area)
        {
            if (!NetworkServer.active)
            {
                Debug.LogError("Called PlaceTowerOnGrid from a client");
                return;
            }

            area.position = mapGrid.WorldToGridPosition(position);
            
            var cell = new GridCell() {State = GridCellState.Occupied};
            towerGrid.SetAreaToCell(area, cell);
            
            messagingManager.SendNamedMessageToAll(new SetGridAreaMessage(){Type = GridType.Tower, Area = area, Cell = cell});
        }
        

        public void EmptyGridArea(GridType gridType, GridArea gridArea)
        {
            if (!NetworkServer.active)
            {
                Debug.LogError("Called PlaceTowerOnGrid from a client");
                return;
            }
            
            var cell = new GridCell() {State = GridCellState.Empty};
            switch (gridType)
            {
                case GridType.Map:
                    mapGrid.SetAreaToCell(gridArea, cell);
                    break;
                case GridType.Tower:
                    towerGrid.SetAreaToCell(gridArea, cell);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            messagingManager.SendNamedMessageToAll(new SetGridAreaMessage(){Type = gridType, Area = gridArea, Cell = cell});
        }

        #endregion

        #region Client

        void Handle_SetGridArea(NetworkConnection sender, Stream stream)
        {
            var message = MessagePackSerializer.Deserialize<SetGridAreaMessage>(stream);

            switch (message.Type)
            {
                case GridType.Map:
                    mapGrid.SetAreaToCell(message.Area, message.Cell);
                    break;
                case GridType.Tower:
                    towerGrid.SetAreaToCell(message.Area, message.Cell);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        

        #endregion
        
        // [Server]
        // public void EmptyGridArea(GridType gridType, GridArea gridArea)
        // {
        //     switch (gridType)
        //     {
        //         case GridType.Map:
        //             mapGrid.SetAreaCellState(gridArea, GridCellState.Empty);
        //             break;
        //         case GridType.Tower:
        //             towerGrid.SetAreaCellState(gridArea, GridCellState.Empty);
        //             break;
        //     }
        //
        //     // Rpc_EmptyGridArea(gridType, gridArea);
        // }

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