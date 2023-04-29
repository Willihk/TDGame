using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using MessagePack;
using TDGame.Cursor;
using TDGame.Events;
using TDGame.Network.Components;
using TDGame.Network.Components.Messaging;
using TDGame.Player;
using TDGame.PrefabManagement;
using TDGame.Systems.Building.Messages.Client;
using TDGame.Systems.Building.Messages.Server;
using TDGame.Systems.Economy;
using TDGame.Systems.Grid.Data;
using TDGame.Systems.Grid.InGame;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using Hash128 = Unity.Entities.Hash128;

namespace TDGame.Systems.Building
{
    public class PlacementManager : MonoBehaviour
    {
        [SerializeField]
        private PrefabManager prefabManager;
        
        [SerializeField]
        private LocalCursorState cursorState;

        [SerializeField]
        private LocalPlayer localPlayer;

        [SerializeField]
        private BuildingManager buildingManager;

        [SerializeField]
        private Material placementMaterial;

        // Key is playerId, value is placement object
        private Dictionary<int, GameObject> underPlacement = new();

        /// <summary>
        /// Only on the server, used to keep track of which Hash128's is currently in use.
        /// </summary>
        private Dictionary<int, Hash128> serverPlacementTracker = new();

        private PlayerEconomyManager economyManager;
        private NetworkPlayerManager playerManager;
        private GridManager gridManager;
        private BaseMessagingManager messagingManager;

        private Camera referenceCamera;

        private static readonly int IsValidID = Shader.PropertyToID("IsValid");

        private void Start()
        {
            referenceCamera = Camera.main;

            playerManager = NetworkPlayerManager.Instance;
            prefabManager = PrefabManager.Instance;
            gridManager = GridManager.Instance;
            economyManager = PlayerEconomyManager.Instance;
            
            messagingManager = BaseMessagingManager.Instance;

            // Server
            messagingManager.RegisterNamedMessageHandler<StartPlacementRequest>(Handle_StartPlacementRequest);
            messagingManager.RegisterNamedMessageHandler<CancelPlacementRequest>(Handle_CancelPlacementRequest);
            messagingManager.RegisterNamedMessageHandler<ConfirmPlacementRequest>(Handle_ConfirmPlacementRequest);
            messagingManager.RegisterNamedMessageHandler<UpdatePositionRequest>(Handle_UpdatePositionRequest);


            // Client
            messagingManager.RegisterNamedMessageHandler<NewPlacementMessage>(Handle_NewPlacementMessage);
            messagingManager.RegisterNamedMessageHandler<RemovePlacementMessage>(Handle_RemovePlacementMessage);
            messagingManager.RegisterNamedMessageHandler<SetPositionMessage>(Handle_SetPositionMessage);


            EventManager.Instance.onBeginPlacement.EventListeners += OnBeginPlacing;
        }

        private void Update()
        {
            // Check grid state

            foreach (var item in underPlacement.Values)
            {
                var valid = gridManager.CanPlaceTower(item.GetComponent<GridAreaController>().CalculateArea());

                foreach (var childRenderer in item.GetComponentsInChildren<Renderer>())
                {
                    childRenderer.material.SetInt(IsValidID, valid ? 1 : 0);
                }
            }


            // Local stuff

            if (!referenceCamera)
            {
                referenceCamera = Camera.main;
                return;
            }

            if (cursorState.State != CursorState.Placing || !underPlacement.ContainsKey(localPlayer.playerId))
                return;


            if (Input.GetMouseButtonDown(1))
            {
                cursorState.State = CursorState.None;
                CancelPlacement();
            }

            var ray = referenceCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("TowerPlacementArea")))
            {
                float gridOffset = 1f / GridManager.Instance.cellSize;
                var hitPoint = math.round((float3)hit.point * gridOffset) / gridOffset;

                var newPos = new Vector3(hitPoint.x, transform.position.y, hitPoint.z);

                if (underPlacement[localPlayer.playerId].transform.position != newPos)
                {
                    underPlacement[localPlayer.playerId].transform.position =
                        newPos;
                    messagingManager.SendNamedMessageToServer(new UpdatePositionRequest() { Position = newPos });
                }
            }

            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                // Cursor state is set to none when the server removes the object under placement
                ConfirmPlacement(underPlacement[localPlayer.playerId].transform.position);
            }
        }

        private void ReplaceModelMaterialsRecursive(Transform transform, Material material)
        {
            if (transform.TryGetComponent(out Renderer renderer))
            {
                renderer.materials = new Material[] { material };
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            foreach (Transform child in transform)
            {
                ReplaceModelMaterialsRecursive(child, material);
            }
        }

        private async UniTaskVoid NewPlacement(int playerId, Hash128 guid)
        {
            var prefab = prefabManager.GetPrefab(guid);
            var model = prefab.transform.Find("Model").gameObject;
            
            var spawned = Instantiate(model);
            
            underPlacement.Add(playerId, spawned);
            ReplaceModelMaterialsRecursive(spawned.transform, new Material(placementMaterial));
        }

        #region Server

        void Handle_StartPlacementRequest(TDNetworkConnection sender, Stream stream)
        {
            var message = MessagePackSerializer.Deserialize<StartPlacementRequest>(stream);

            int id = playerManager.GetPlayerId(sender);
            
            
            if (serverPlacementTracker.ContainsKey(id))
            {
                // Cancel placement
                messagingManager.SendNamedMessageToAll(new RemovePlacementMessage() { PlayerId = id });
                serverPlacementTracker.Remove(id);
            }

            serverPlacementTracker.Add(id, message.AssetGuid);

            messagingManager.SendNamedMessageToAll(new NewPlacementMessage
                { AssetGuid = message.AssetGuid, PlayerId = id });
        }

        void Handle_ConfirmPlacementRequest(TDNetworkConnection sender, Stream stream)
        {
            GridArea GetArea(Hash128 guid, Vector3 position, GridManager grid)
            {
                var prefab = prefabManager.GetPrefab(guid);
                var area = prefab.transform.Find("Model").GetComponent<GridAreaController>().area;

                var offset = new int2(area.width, area.height) / 2;
                area.position = grid.towerGrid.WorldToGridPosition(position) - offset;

                return area;
            }

            Debug.Log("confirmed placement for player: " + sender);
            var message = MessagePackSerializer.Deserialize<ConfirmPlacementRequest>(stream);

            var playerId = playerManager.GetPlayerId(sender);

            var assetToBuild = serverPlacementTracker[playerId];

            var towerDetail = prefabManager.GetTowerDetails(assetToBuild);
            if (towerDetail == null)
            {
                Handle_CancelPlacementRequest(sender, null);
                Debug.LogWarning("Trying to place invalid tower!");
                return;
            }

            if (!economyManager.GetEconomy(playerId).CanAfford(towerDetail.Price))
            {
                Handle_CancelPlacementRequest(sender, null);
                Debug.LogWarning("Cannot afford tower!");
                return;
            }

            var area = GetArea(assetToBuild, message.Position, gridManager);
            if (!gridManager.CanPlaceTower(area))
            {
                Debug.LogWarning("Not valid placement!");
                return;
            }

            economyManager.GetEconomy(playerId).Purchase(towerDetail.Price);
            economyManager.SyncEconomies();

            Handle_CancelPlacementRequest(sender, null);
            buildingManager.Server_BuildBuilding(assetToBuild, area);
        }

        void Handle_CancelPlacementRequest(TDNetworkConnection sender, Stream stream)
        {
            var id = playerManager.GetPlayerId(sender);

            serverPlacementTracker.Remove(id);

            messagingManager.SendNamedMessageToAll(new RemovePlacementMessage { PlayerId = id });
        }

        void Handle_UpdatePositionRequest(TDNetworkConnection sender, Stream stream)
        {
            var message = MessagePackSerializer.Deserialize<UpdatePositionRequest>(stream);

            var playerId = playerManager.GetPlayerId(sender);
            
            if (!underPlacement.ContainsKey(playerId))
                return;

            var targetObject = underPlacement[playerId];

            if (targetObject.transform.position == message.Position && playerId != localPlayer.playerId)
                return;

            targetObject.transform.position = message.Position;

            messagingManager.SendNamedMessageToAll(
                new SetPositionMessage { Id = playerId, Position = message.Position });
        }

        #endregion

        #region Client

        // When local player clicks on hotbar icon to begin placing a building.
        public void OnBeginPlacing(Hash128 guid)
        {
            messagingManager.SendNamedMessageToServer(new StartPlacementRequest() { AssetGuid = guid });
        }

        private void CancelPlacement()
        {
            messagingManager.SendNamedMessageToServer(new CancelPlacementRequest());
        }

        private void ConfirmPlacement(Vector3 position)
        {
            messagingManager.SendNamedMessageToServer(new ConfirmPlacementRequest() { Position = position });
        }

        void Handle_NewPlacementMessage(TDNetworkConnection sender, Stream stream)
        {
            var message = MessagePackSerializer.Deserialize<NewPlacementMessage>(stream);
            NewPlacement(message.PlayerId, message.AssetGuid).Forget();
        }

        void Handle_RemovePlacementMessage(TDNetworkConnection sender, Stream stream)
        {
            var message = MessagePackSerializer.Deserialize<RemovePlacementMessage>(stream);

            if (message.PlayerId == localPlayer.playerId && cursorState.State == CursorState.Placing)
            {
                // Could cause undefined behaviour if theres network delay
                cursorState.State = CursorState.None;
            }

            if (underPlacement.TryGetValue(message.PlayerId, out var handle))
            {
                Destroy(handle);
                underPlacement.Remove(message.PlayerId);
            }
        }

        void Handle_SetPositionMessage(TDNetworkConnection sender, Stream stream)
        {
            var message = MessagePackSerializer.Deserialize<SetPositionMessage>(stream);


            if (underPlacement.TryGetValue(message.Id, out var target))
            {
                target.transform.position = message.Position;
            }
        }

        #endregion
    }
}