using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TDGame.Building
{
    [CreateAssetMenu(fileName = "BuildingList", menuName = "Data/Building/BuildingList", order = 0)]
    public class BuildingList : ScriptableObject
    {
        [SerializeField]
        private List<GameObject> prefabs;
        
        public GameObject GetBuilding(string name)
        {
            return prefabs.First(x => x.name == name);
        }

        public GameObject GetBuilding(int index)
        {
            return prefabs[index];
        }

        public int GetIndexOfBuildingName(string name)
        {
            return prefabs.Select(x => x.name).ToList().IndexOf(name);
        }

        public List<GameObject> GetBuildings()
        {
            return prefabs;
        }
    }
}