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

            // for (int i = 0; i < buildings.Length; i++)
            // {
            //     var entryObject = Instantiate(entryPrefab, content);
            //     if (buildings[i].TryGetComponent(out BaseNetworkedTower component))
            //         entryObject.GetComponent<BuildingListEntry>().Initialize(buildings[i].name, component.DisplayInfo.Name, component.price);
            //     else
            //         entryObject.GetComponent<BuildingListEntry>().Initialize(buildings[i].name, buildings[i].name, 0);
            // }
        }
        
        private async UniTaskVoid GenerateEntries(IReadOnlyCollection<TowerDetails> entries)
        {
            foreach (var item in entries)
            {
                var handle = Addressables.InstantiateAsync(entryPrefab);
                handles.Add(handle);

                var entryObject = await handle;
                
            }
            
            // for (var i = 0; i < entries.Count; i++)
            // {
            //     var id = entries.ElementAt(i);
            //     var handle = Addressables.InstantiateAsync(entryPrefab);
            //     handles.Add(handle);
            //
            //     var entry = await handle;
            //     entry.GetComponent<LobbyPlayerListEntry>().Initialize("Player" + id, true);
            //     entry.SetActive(true);
            //     entry.transform.SetParent(content);
            //
            //     entry.transform.localScale = Vector3.one;
            // }
        }
    }
}