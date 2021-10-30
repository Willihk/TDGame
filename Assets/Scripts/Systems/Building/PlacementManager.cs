using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using MessagePack;
using TDGame.Cursor;
using TDGame.Network.Components;
using TDGame.Network.Components.Messaging;
using TDGame.Player;
using TDGame.Systems.Building.Messages.Client;
using TDGame.Systems.Building.Messages.Server;
using TDGame.Systems.Grid.InGame;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TDGame.Systems.Building
{
    public class PlacementManager : MonoBehaviour
    {
        [SerializeField]
        private LocalCursorState cursorState;

        [SerializeField]
        private LocalPlayer localPlayer;

        [SerializeField]
        private BuildingManager buildingManager;


        // Key is playerId, value is placement object
        private Dictionary<int, GameObject> underPlacement = new Dictionary<int, GameObject>();

        private Dictionary<int, AsyncOperationHandle<GameObject>> handles =
            new Dictionary<int, AsyncOperationHandle<GameObject>>();

        /// <summary>
        /// Only on the server, used to keep track of which AssetReference's is currently in use.
        /// </summary>
        private Dictionary<int, AssetReference> serverPlacementTracker = new Dictionary<int, AssetReference>();

        private NetworkPlayerManager playerManager;

        private GridManager gridManager;

        private BaseMessagingManager messagingManager;

        private Camera referenceCamera;

        private void Start()
        {
            referenceCamera = Camera.main;

            playerManager = NetworkPlayerManager.Instance;
            
            gridManager = GridManager.Instance;


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
        }

        private void Update()
        {
            if (referenceCamera == null)
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
                    messagingManager.SendNamedMessageToServer(new UpdatePositionRequest(){Position = newPos});
                }
            }

            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                cursorState.State = CursorState.None;
                ConfirmPlacement(underPlacement[localPlayer.playerId].transform.position);
            }
        }


        private async UniTaskVoid NewPlacement(int playerId, AssetReference reference)
        {
            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(reference);

            handles.Add(playerId, handle);
            var prefab = await handle;
            var model = prefab.transform.Find("Model").gameObject;

            var spawned = Instantiate(model);
            underPlacement.Add(playerId, spawned);
        }

        #region Server

        void Handle_StartPlacementRequest(NetworkConnection sender, Stream stream)
        {
            var message = MessagePackSerializer.Deserialize<StartPlacementRequest>(stream);

            int id = playerManager.GetPlayerId(sender);
            var assetReference = new AssetReference(message.AssetGuid);
            if (serverPlacementTracker.ContainsKey(id))
            {
                // Cancel placement
                messagingManager.SendNamedMessageToAll(new RemovePlacementMessage() { PlayerId = id });
                serverPlacementTracker.Remove(id);
            }

            serverPlacementTracker.Add(id, assetReference);

            messagingManager.SendNamedMessageToAll(new NewPlacementMessage
                { AssetGuid = message.AssetGuid, PlayerId = id });
        }

        void Handle_ConfirmPlacementRequest(NetworkConnection sender, Stream stream)
        {
            Debug.Log("confirmed placement for player: " + sender.id);
            var message = MessagePackSerializer.Deserialize<ConfirmPlacementRequest>(stream);

            var assetToBuild = serverPlacementTracker[playerManager.GetPlayerId(sender)];


            buildingManager.Server_BuildBuilding(assetToBuild, message.Position);

            Handle_CancelPlacementRequest(sender, null);
        }

        void Handle_CancelPlacementRequest(NetworkConnection sender, Stream stream)
        {
            var id = playerManager.GetPlayerId(sender);

            serverPlacementTracker.Remove(id);

            messagingManager.SendNamedMessageToAll(new RemovePlacementMessage { PlayerId = id });
        }

        void Handle_UpdatePositionRequest(NetworkConnection sender, Stream stream)
        {
            var message = MessagePackSerializer.Deserialize<UpdatePositionRequest>(stream);

            var playerId = playerManager.GetPlayerId(sender);
            var targetObject = underPlacement[playerId];

            if (targetObject.transform.position == message.Position)
                return;
            
            targetObject.transform.position = message.Position;
            messagingManager.SendNamedMessageToAll(new SetPositionMessage { Id = playerId, Position = message.Position});
        }

        #endregion

        #region Client

        // When local player clicks on hotbar icon to begin placing a building.
        public void OnBeginPlacing(string guid)
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

        void Handle_NewPlacementMessage(NetworkConnection sender, Stream stream)
        {
            var message = MessagePackSerializer.Deserialize<NewPlacementMessage>(stream);
            NewPlacement(message.PlayerId, new AssetReference(message.AssetGuid)).Forget();
        }

        void Handle_RemovePlacementMessage(NetworkConnection sender, Stream stream)
        {
            var message = MessagePackSerializer.Deserialize<RemovePlacementMessage>(stream);
            if (handles.TryGetValue(message.PlayerId, out AsyncOperationHandle<GameObject> handle))
            {
                Destroy(underPlacement[message.PlayerId]);
                
                Addressables.Release(handle);
                handles.Remove(message.PlayerId);
                underPlacement.Remove(message.PlayerId);
            }
        }

        void Handle_SetPositionMessage(NetworkConnection sender, Stream stream)
        {
            var message = MessagePackSerializer.Deserialize<SetPositionMessage>(stream);


            if (underPlacement.TryGetValue(message.Id, out GameObject target))
            {
                target.transform.position = message.Position;
            }
        }

        #endregion
    }
}