using System.Collections;
using System.Collections.Generic;
using Mirror;
using TDGame.Enemy.Base;
using TDGame.Enemy.Data;
using UnityEngine;

namespace TDGame.Enemy
{
    public class EnemyWaveController : NetworkBehaviour
    {
        [SerializeField]
        private EnemyList enemyList;

        [SerializeField]
        private Transform enemyHolder;

        [SerializeField]
        private List<Vector3> waypoints;

        public override void OnStartServer()
        {
            base.OnStartServer();
            StartCoroutine(nameof(SpawnTestEnemies));
        }

        IEnumerator SpawnTestEnemies()
        {
            var prefab = enemyList.GetEnemy(0);
            while (true)
            {
                SpawnEnemy(prefab);
                yield return new WaitForSeconds(1);
            }
        }

        [Server]
        void SpawnEnemy(GameObject prefab)
        {
            var enemyObject = Instantiate(prefab, enemyHolder);
            enemyObject.GetComponent<NetworkedEnemy>().Setup(waypoints);
            
            NetworkServer.Spawn(enemyObject);
        }
    }
}