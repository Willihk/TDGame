using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using MessagePack;
using TDGame.Network.Components;
using TDGame.Network.Components.Messaging;
using TDGame.Systems.Building.Messages.Client;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TDGame.Systems.Building
{
    public class BuildingManager : MonoBehaviour
    {
        // Key is playerId, value is placement object
        private Dictionary<int, GameObject> underPlacement = new Dictionary<int, GameObject>();
        private Dictionary<int, AsyncOperationHandle<GameObject>> handles = new Dictionary<int, AsyncOperationHandle<GameObject>>();

        private NetworkPlayerManager playerManager;

        private void Start()
        {
            playerManager = NetworkPlayerManager.Instance;
        }


        private async UniTaskVoid NewPlacement(int playerId, AssetReference reference)
        {
            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(reference);

            handles.Add(playerId, handle);
            var prefab = await handle;
            var model = prefab.transform.Find("Model").gameObject;


        }

        void Handle_StartPlacement(NetworkConnection sender, Stream stream)
        {
            var message = MessagePackSerializer.Deserialize<StartPlacementMessage>(stream);
            
        }
        
        void Handle_CancelPlacement(NetworkConnection sender, Stream stream)
        {
            var id = playerManager.GetPlayerId(sender);

        }
    }
}