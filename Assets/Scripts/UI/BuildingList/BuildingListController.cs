using System;
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
                entryObject.GetComponent<BuildingListEntry>().Initialize(buildings[i].name,buildings[i].name);
            }
        }
    }
}