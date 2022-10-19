using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using MessagePack;
using TDGame.Network.Components.Messaging;
using TDGame.Systems.Enemy.Components.Spawning;
using TDGame.Systems.Enemy.Systems.Messages;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TDGame.Systems.Enemy.Systems
{
    public class NetworkedSpawnManager : MonoBehaviour
    {
        public static NetworkedSpawnManager instance;

        private Dictionary<string, AsyncOperationHandle<GameObject>> openHandles = new ();

        private Dictionary<string, Entity> openPrefabs = new();
        
        private EntityManager entityManager;
        private BlobAssetStore assetStore;
        BaseMessagingManager messagingManager;

        private void Awake()
        {
            instance = this;
            assetStore = new BlobAssetStore();
        }

        private void Start()
        {
            messagingManager = BaseMessagingManager.Instance;
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            messagingManager.RegisterNamedMessageHandler<SpawnEnemyMessage>(Handle_SpawnEnemyMessage);
        }

        private void OnDestroy()
        {
            foreach (var handle in openHandles)
            {
                Addressables.Release(handle);    
            }
        }


        public void SpawnEnemy(string id)
        {
            messagingManager.SendNamedMessageToAll(new SpawnEnemyMessage(){Id = id});
        }

        async UniTask LoadNewPrefab(string id)
        {
            var reference = new AssetReference(id);
            var handle = Addressables.LoadAssetAsync<GameObject>(reference);
                
            await handle;
                
            var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, assetStore);
            var prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(handle.Result, settings);
            
            openHandles.Add(id, handle);
            openPrefabs.Add(id, prefab);
        }

        void Handle_SpawnEnemyMessage(NetworkConnection sender, Stream stream)
        {
            var message = MessagePackSerializer.Deserialize<SpawnEnemyMessage>(stream);

            if (!openHandles.ContainsKey(message.Id))
            {
                UniTask.Create(async () =>
                {
                    await LoadNewPrefab(message.Id);
                    Handle_SpawnEnemyMessage(sender, stream);
                });
                return;
            }
            var prefab = openPrefabs[message.Id];
            if (prefab == Entity.Null)
                return;
            
            var data = new SpawnEnemy { prefab =  default}; //TODO: Legacy code
            
            var entity = entityManager.CreateEntity();
            entityManager.AddComponent<SpawnEnemy>(entity);
            
            entityManager.SetComponentData(entity, data);
        }
    }
}