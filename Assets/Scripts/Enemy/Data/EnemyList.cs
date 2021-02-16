using System.Collections.Generic;
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
    }
}