using System.IO;
using Cysharp.Threading.Tasks;
using MessagePack;
using TDGame.Network.Components.Messaging;
using TDGame.PrefabManagement;
using TDGame.Systems.Building.Messages.Server;
using TDGame.Systems.Grid.Data;
using TDGame.Systems.Grid.InGame;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Hash128 = Unity.Entities.Hash128;

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

        private void BuildTower(Hash128 guid, float3 position)
        {
            var prefab = PrefabManager.Instance.GetEntityPrefab(guid);
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            
            var entity = entityManager.Instantiate(prefab);
            
            var localTransform = entityManager.GetComponentData<LocalTransform>(entity);
            localTransform.Position = position;
            entityManager.SetComponentData(entity, localTransform);
        }

        private void Handle_NewBuildingMessage(TDNetworkConnection sender, Stream stream)
        {
            var message = MessagePackSerializer.Deserialize<NewBuildingMessage>(stream);
            

            BuildTower(message.AssetGuid, message.Position);
        }

        #region Server

        public void Server_BuildBuilding(Hash128 guid, GridArea area)
        {
            var gridManager = GridManager.Instance;
            gridManager.PlaceTowerOnGrid(null, area);

            messagingManager.SendNamedMessageToAll(new NewBuildingMessage
            {
                AssetGuid = guid,
                Position = gridManager.towerGrid.GridToWorldPosition(area.position +
                                                                     new int2(area.width, area.height) / 2)
            });
        }

        #endregion
    }
}