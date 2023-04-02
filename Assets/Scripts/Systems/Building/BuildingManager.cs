using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using MessagePack;
using TDGame.Network.Components.Messaging;
using TDGame.Systems.Building.Messages.Server;
using TDGame.Systems.Grid.Data;
using TDGame.Systems.Grid.InGame;
using Unity.Entities;
using Unity.Mathematics;
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
            // spawned.AddComponent<ConvertToEntity>();
        }

        private void Handle_NewBuildingMessage(TDNetworkConnection sender, Stream stream)
        {
            var message = MessagePackSerializer.Deserialize<NewBuildingMessage>(stream);

            var assetReference = new AssetReference(message.AssetGuid);

            BuildTower(assetReference, message.Position).Forget();
        }

        #region Server

        public void Server_BuildBuilding(AssetReference assetReference, GridArea area)
        {
            var gridManager = GridManager.Instance;
            gridManager.PlaceTowerOnGrid(null, area);

            messagingManager.SendNamedMessageToAll(new NewBuildingMessage
            {
                AssetGuid = assetReference.AssetGUID,
                Position = gridManager.towerGrid.GridToWorldPosition(area.position +
                                                                     new int2(area.width, area.height) / 2)
            });
        }

        #endregion
    }
}