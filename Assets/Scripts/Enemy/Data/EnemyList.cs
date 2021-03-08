using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TDGame.Enemy.Data
{
    [CreateAssetMenu(fileName = "EnemyList", menuName = "Data/Enemy/EnemyList", order = 0)]
    public class EnemyList : ScriptableObject
    {
        [SerializeField]
        private List<GameObject> prefabs;

        public IEnumerable<GameObject> GetEnemies()
        {
            return prefabs;
        }

        public GameObject GetEnemy(int index)
        {
            return prefabs[index];
        }
        
        public GameObject GetEnemy(string prefabName)
        {
            return prefabs.First(x => x.name == prefabName);
        }
    }
}