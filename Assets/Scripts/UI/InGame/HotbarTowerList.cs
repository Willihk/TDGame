using TDGame.Systems.Tower.Graph;
using UnityEngine;

namespace TDGame.UI.InGame
{
    public class HotbarTowerList : MonoBehaviour
    {
        [SerializeField]
        private TowerGraph towerGraph;
        
        [SerializeField]
        private GameObject entryPrefab;

        [SerializeField]
        private Transform content;
        
        private void Start()
        {
            var buildings = towerGraph.GetHotbarTowers();

            foreach (var item in buildings)
            {
                var entryObject = Instantiate(entryPrefab, content);
                entryObject.GetComponent<BuildingListEntry>().Initialize(item.name, item.Name, item.Price);
            }
        }
    }
}