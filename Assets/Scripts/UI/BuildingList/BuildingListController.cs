using System;
using TDGame.Systems.Tower.Base;
using UnityEngine;

namespace TDGame.UI.BuildingList
{
    public class BuildingListController : MonoBehaviour
    {
        [SerializeField]
        private Building.BuildingList buildingList;

        [SerializeField]
        private GameObject entryPrefab;

        [SerializeField]
        private Transform content;


        private void Start()
        {
            var buildings = this.buildingList.GetBuildings();

            for (int i = 0; i < buildings.Count; i++)
            {
                var entryObject = Instantiate(entryPrefab, content);
                if (buildings[i].TryGetComponent(out BaseNetworkedTower component))
                    entryObject.GetComponent<BuildingListEntry>().Initialize(buildings[i].name, component.DisplayInfo.Name);
                else
                    entryObject.GetComponent<BuildingListEntry>().Initialize(buildings[i].name, buildings[i].name);
            }
        }
    }
}