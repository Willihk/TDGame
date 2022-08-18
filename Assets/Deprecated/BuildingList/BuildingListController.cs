using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TDGame.Systems.Tower.Data;
using TDGame.Systems.Tower.Graph;
using TDGame.UI.Lobby;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TDGame.UI.InGame.BuildingList
{
    public class BuildingListController : MonoBehaviour
    {
        [SerializeField]
        private TowerGraph towerGraph;

        [SerializeField]
        private AssetReferenceT<GameObject> entryPrefab;

        [SerializeField]
        private Transform content;
        
        private List<AsyncOperationHandle<GameObject>> handles = new List<AsyncOperationHandle<GameObject>>();

        private void Start()
        {
            var buildings = towerGraph.GetHotbarTowers().ToArray();

            GenerateEntries(buildings).Forget();
        }
        
        private async UniTaskVoid GenerateEntries(IEnumerable<TowerDetails> entries)
        {
            foreach (var item in entries)
            {
                var handle = Addressables.InstantiateAsync(entryPrefab);
                handles.Add(handle);

                var entryObject = await handle;
                entryObject.SetActive(true);
                entryObject.transform.SetParent(content);
                
                entryObject.transform.localScale = Vector3.one;

                var listEntry = entryObject.GetComponent<BuildingListEntry>();
                listEntry.Initialize(item);
            }
        }
    }
}