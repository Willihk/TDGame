using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using MessagePack;
using TDGame.Network.Components;
using TDGame.Network.Components.Messaging;
using TDGame.Systems.Building.Messages.Client;
using TDGame.Systems.Building.Messages.Server;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TDGame.Systems.Building
{
    public class BuildingManager : MonoBehaviour
    {
        // Key is playerId, value is placement object
        private Dictionary<int, GameObject> underPlacement = new Dictionary<int, GameObject>();

        private Dictionary<int, AsyncOperationHandle<GameObject>> handles =
            new Dictionary<int, AsyncOperationHandle<GameObject>>();

        /// <summary>
        /// Only on the server, used to keep track of which AssetReference's is currently in use.
        /// </summary>
        private Dictionary<int, AssetReference> serverPlacementTracker = new Dictionary<int, AssetReference>();

        private NetworkPlayerManager playerManager;

        private BaseMessagingManager messagingManager;

        private void Start()
        {
            playerManager = NetworkPlayerManager.Instance;


            messagingManager = BaseMessagingManager.Instance;

            // Server
            messagingManager.RegisterNamedMessageHandler<StartPlacementRequest>(Handle_StartPlacementRequest);
            messagingManager.RegisterNamedMessageHandler<CancelPlacementRequest>(Handle_CancelPlacementRequest);

            // Client
            messagingManager.RegisterNamedMessageHandler<NewPlacementMessage>(Handle_NewPlacementMessage);
            messagingManager.RegisterNamedMessageHandler<RemovePlacementMessage>(Handle_RemovePlacementMessage);
        }


        private async UniTaskVoid NewPlacement(int playerId, AssetReference reference)
        {
            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(reference);

            handles.Add(playerId, handle);
            var prefab = await handle;
            var model = prefab.transform.Find("Model").gameObject;

            Instantiate(model);
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
                messagingManager.SendNamedMessageToAll(new RemovePlacementMessage() {PlayerId = id});
                serverPlacementTracker.Remove(id);
            }

            serverPlacementTracker.Add(id, assetReference);

            messagingManager.SendNamedMessageToAll(new NewPlacementMessage
                {AssetGuid = message.AssetGuid, PlayerId = id});
        }

        void Handle_CancelPlacementRequest(NetworkConnection sender, Stream stream)
        {
            var id = playerManager.GetPlayerId(sender);

            serverPlacementTracker.Remove(id);

            messagingManager.SendNamedMessageToAll(new RemovePlacementMessage {PlayerId = id});
        }

        #endregion

        #region Client

        // When local player clicks on hotbar icon to begin placing a building.
        public void OnBeginPlacing(string guid)
        {
            messagingManager.SendNamedMessageToServer(new StartPlacementRequest() {AssetGuid = guid});
        }

        public void CancelPlacement()
        {
            messagingManager.SendNamedMessageToServer(new CancelPlacementRequest());
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
                Addressables.ReleaseInstance(handle);
                handles.Remove(message.PlayerId);
            }
        }

        #endregion
    }
}