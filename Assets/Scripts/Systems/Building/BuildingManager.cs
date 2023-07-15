using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using MessagePack;
using TDGame.Network.Components.Messaging;
using TDGame.PrefabManagement;
using TDGame.Systems.Building.Messages.Server;
using TDGame.Systems.Grid.Data;
using TDGame.Systems.Grid.InGame;
using TDGame.Systems.Tower.Graph.Data;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Hash128 = Unity.Entities.Hash128;
using Random = UnityEngine.Random;

namespace TDGame.Systems.Building
{
    public class BuildingManager : MonoBehaviour
    {
        public static BuildingManager Instance;

        private void Awake()
        {
            Instance = this;
        }

        private BaseMessagingManager messagingManager;

        // key is id, value is prefab hash
        private Dictionary<int, Hash128> spawned = new ();

        private void Start()
        {
            messagingManager = BaseMessagingManager.Instance;

            messagingManager.RegisterNamedMessageHandler<NewBuildingMessage>(Handle_NewBuildingMessage);
            messagingManager.RegisterNamedMessageHandler<RemoveBuildingMessage>(Handle_RemoveBuildingMessage);
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
            
            spawned.Add(message.ID, message.AssetGuid);
            BuildTower(message.AssetGuid, message.Position);
        }
        
        private void Handle_RemoveBuildingMessage(TDNetworkConnection sender, Stream stream)
        {
            var message = MessagePackSerializer.Deserialize<RemoveBuildingMessage>(stream);
            
            spawned.Remove(message.Id);

            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var query = entityManager.CreateEntityQuery(typeof(NetworkId));
            var entities = query.ToEntityArray(Allocator.Temp);

            foreach (var entity in entities)  
            {
                if (entityManager.GetComponentData<NetworkId>(entity).Value == message.Id)
                {
                    entityManager.DestroyEntity(entity);
                    break;
                }  
            }
        }

        public bool GetHashById(int id, out Hash128 hash)
        {
            return spawned.TryGetValue(id, out hash);
        }

        public bool GetDetailsOfTower(int id, out TowerDetails towerDetails)
        {
            if (!GetHashById(id, out var hash))
            {
                towerDetails = null;   
                return false;
            }
            
            towerDetails = PrefabManager.Instance.GetTowerDetails(hash);
            return true;

        }

        #region Server

        public void Server_BuildBuilding(Hash128 guid, GridArea area)
        {
            int id = Random.Range(int.MinValue, int.MaxValue);
            
            var gridManager = GridManager.Instance;
            gridManager.PlaceTowerOnGrid(id, area);

            messagingManager.SendNamedMessageToAll(new NewBuildingMessage
            {
                AssetGuid = guid,
                Position = gridManager.towerGrid.GridToWorldPosition(area.position +
                                                                     new int2(area.width, area.height) / 2),
                ID = id
            });
        } 
        public void Server_RemoveBuilding(int id)
        {
            var gridManager = GridManager.Instance;
            var towerArea = gridManager.towerGrid.GetAreaOfOccupier(id);
            gridManager.EmptyGridArea(GridType.Tower, towerArea);
            
            messagingManager.SendNamedMessageToAll(new RemoveBuildingMessage()
            {
                Id = id
            });
        }

        #endregion
    }
}