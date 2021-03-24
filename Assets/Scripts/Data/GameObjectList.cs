using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TDGame.Data
{
    [CreateAssetMenu(fileName = "GameObjectList", menuName = "Data/GameObjectList/GameObjectList", order = 0)]
    public class GameObjectList : ScriptableObject
    {
        [SerializeField]
        private List<GameObject> prefabs;
        
        public GameObject GetGameObject(string name)
        {
            return prefabs.First(x => x.name == name);
        }

        public GameObject GetGameObject(int index)
        {
            return prefabs[index];
        }

        public int GetIndexOfGameObjectName(string name)
        {
            return prefabs.Select(x => x.name).ToList().IndexOf(name);
        }

        public List<GameObject> GetGameObjects()
        {
            return prefabs;
        }
    }
}