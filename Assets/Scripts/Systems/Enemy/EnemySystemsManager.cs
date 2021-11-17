using System;
using Cysharp.Threading.Tasks;
using TDGame.Systems.Enemy.Components.Spawning;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TDGame.Systems.Enemy
{
    public class EnemySystemsManager : MonoBehaviour
    {
        [SerializeField]
        private float delay = 0.5f;
        
        private EntityManager entityManager;
        private BlobAssetStore assetStore;
        private Entity exampleEnemyPrefab;

        private float nextSpawn;
        
        private async void Start()
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            assetStore = new BlobAssetStore();
            var keys =
                await Addressables.LoadResourceLocationsAsync(
                    "ExampleEnemy.prefab"); // Handle might need to be released -who knows???

            var handle = Addressables.LoadAssetAsync<GameObject>(keys[0]);

            var enemyObject = await handle;

            var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, assetStore);

            exampleEnemyPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(enemyObject, settings);
        }

        private void Update()
        {
            if (nextSpawn > Time.time || exampleEnemyPrefab == Entity.Null)
                return;
            
            Entity newEntity = entityManager.CreateEntity();
            entityManager.AddComponent<SpawnEnemy>(newEntity);
            entityManager.SetComponentData(newEntity, new SpawnEnemy() { prefab = exampleEnemyPrefab });
            nextSpawn = Time.time + delay;
        }
    }
}