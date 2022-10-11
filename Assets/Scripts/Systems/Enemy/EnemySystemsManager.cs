using System;
using Cysharp.Threading.Tasks;
using TDGame.Systems.Enemy.Components.Spawning;
using TDGame.Systems.Enemy.Systems;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace TDGame.Systems.Enemy
{
    public class EnemySystemsManager : MonoBehaviour
    {
        [SerializeField]
        private AssetReferenceGameObject prefabReference;
        [SerializeField]
        private float delay = 0.5f;
        
        private BlobAssetStore assetStore;
        private IResourceLocation handle;
        
        private float nextSpawn;
        
        private void Update()
        {
            if (nextSpawn > Time.time)
                return;

            nextSpawn = Time.time + delay;
            NetworkedSpawnManager.instance.SpawnEnemy(prefabReference.AssetGUID);
        }
    }
}