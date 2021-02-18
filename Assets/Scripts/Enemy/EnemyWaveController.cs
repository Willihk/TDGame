using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using TDGame.Enemy.Base;
using TDGame.Enemy.Data;
using TDGame.Map;
using TDGame.Systems.Targeting.Data;
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
        private NetworkedMapController mapController;
        
        [SerializeField]
        private List<Vector3> waypoints;

        public void OnMapLoaded()
        {
            if (!isServer)
                return;

            waypoints = mapController.GetWaypoints().Select(x => x.position).ToList();
            StartCoroutine(nameof(SpawnTestEnemies));
        }

        IEnumerator SpawnTestEnemies()
        {
            var prefab = enemyList.GetEnemy(0);
            while (true)
            {
                SpawnEnemy(prefab);
                yield return new WaitForSeconds(0.1f);
            }
        }

        [Server]
        void SpawnEnemy(GameObject prefab)
        {
            GameObject enemyObject = Instantiate(prefab, enemyHolder);
            enemyObject.GetComponent<NetworkedEnemy>().Setup(waypoints);

            enemyObject.transform.position = waypoints[0];
            
            NetworkServer.Spawn(enemyObject);
            
            EnemyTargetsController.Instance.targets.Add(enemyObject);
        }
    }
}