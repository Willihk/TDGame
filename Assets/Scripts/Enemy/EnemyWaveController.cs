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
            var boss = enemyList.GetEnemy(1);
            CurrentWave++;

            int waveEnemyCount = (int)(5 * Mathf.Sqrt(Mathf.Pow(CurrentWave, 3)));
            float spawnDelay = Mathf.Max(5 / CurrentWave, 0.1f);
            Queue<WaveCommand> commands = new Queue<WaveCommand>();

            yield return new WaitForSeconds(6f);

            switch (CurrentWave)
            {
                case 10:
                    commands.Enqueue(new SpawnPrefab(boss, enemyHolder, waypoints[0], waypoints));
                    break;
                default:
                    for (int i = 0; i < waveEnemyCount; i++)
                    {
                        commands.Enqueue(new SpawnPrefab(prefab, enemyHolder, waypoints[0], waypoints));
                    }
                    break;
            }

            while (commands.Count > 0)
            {
                commands.Dequeue().Execute();
                yield return new WaitForSeconds(spawnDelay);
            }
            
            yield return new WaitForSeconds(1f);

            AwaitingNextWave = true;
        }
    }
}