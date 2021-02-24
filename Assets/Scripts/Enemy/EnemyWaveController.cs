using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using TDGame.Command.Implementations.Wave;
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
        }

        private int CurrentWave = 0;
        private bool AwaitingNextWave = true;

        void Update()
        {
            if (!isServer)
                return;

            if (EnemyTargetsController.Instance.targets.Count == 0 && AwaitingNextWave)
            {
                AwaitingNextWave = false;
                StartCoroutine(nameof(SpawnTestEnemies));
            }
        }

        IEnumerator SpawnTestEnemies()
        {
            var prefab = enemyList.GetEnemy(0);
            CurrentWave++;
            
            int waveEnemyCount = (int)(5 * Mathf.Sqrt(Mathf.Pow(CurrentWave, CurrentWave)));
            float spawnDelay = 5 / (CurrentWave * CurrentWave);

            Queue<WaveCommand> commands = new Queue<WaveCommand>();

            yield return new WaitForSeconds(1f);

            for (int i = 0; i < waveEnemyCount; i++)
            {
                commands.Enqueue(new SpawnPrefab(prefab, enemyHolder, waypoints[0], waypoints));
            }


            for (int i = 0; i < waveEnemyCount; i++)
            {
                commands.Dequeue().Execute();
                yield return new WaitForSeconds(spawnDelay);
            }
            
            //WaveContent.Clear();
            yield return new WaitForSeconds(1f);

            AwaitingNextWave = true;
        }
    }
}