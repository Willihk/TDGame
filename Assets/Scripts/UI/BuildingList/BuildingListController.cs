using System;
using System.Linq;
using TDGame.Systems.Tower.Base;
using TDGame.Systems.Tower.Graph;
using UnityEngine;

namespace TDGame.UI.BuildingList
{
    public class BuildingListController : MonoBehaviour
    {
        [SerializeField]
        private TowerGraph towerGraph;

        [SerializeField]
        private GameObject entryPrefab;

        [SerializeField]
        private Transform content;

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
    }
}