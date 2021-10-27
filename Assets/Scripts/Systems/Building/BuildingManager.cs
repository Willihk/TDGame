﻿using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using MessagePack;
using TDGame.Network.Components.Messaging;
using TDGame.Systems.Building.Messages.Server;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TDGame.Systems.Building
{
    public class BuildingManager : MonoBehaviour
    {
        
        
        private BaseMessagingManager messagingManager;
        
        private void Start()
        {
            messagingManager = BaseMessagingManager.Instance;
            
            messagingManager.RegisterNamedMessageHandler<NewBuildingMessage>(Handle_NewBuildingMessage);
        }

        private async UniTask BuildTower(AssetReference assetReference, Vector3 position)
        {
            var handle = Addressables.InstantiateAsync(assetReference);

            var spawned = await handle;

            spawned.transform.position = position;
        }

        private void Handle_NewBuildingMessage(NetworkConnection sender, Stream stream)
        {
            var message = MessagePackSerializer.Deserialize<NewBuildingMessage>(stream);

            var assetReference = new AssetReference(message.AssetGuid);
            
            BuildTower(assetReference, message.Position).Forget();
        }
    }
}