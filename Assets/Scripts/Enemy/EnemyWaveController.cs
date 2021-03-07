using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using TDGame.Command.Implementations.Wave;
using TDGame.Enemy.Base;
using TDGame.Enemy.Data;
using TDGame.Events.Base;
using TDGame.Map;
using TDGame.Systems.Targeting.Data;
using UnityEngine;

namespace TDGame.Enemy
{
    public class EnemyWaveController : NetworkBehaviour
    {
        [SerializeField]
        private GameEvent<int> waveChangedEvent;

        [SerializeField]
        private EnemyList enemyList;

        [SerializeField]
        private Transform enemyHolder;

        [SerializeField]
        private NetworkedMapController mapController;

        [SerializeField]
        private List<Vector3> waypoints;

        [SyncVar(hook = nameof(WaveChanged))]
        private int currentWave = 0;

        private bool AwaitingNextWave = true;

        public void OnMapLoaded()
        {
            if (!isServer)
                return;

            waypoints = mapController.GetWaypoints().Select(x => x.position).ToList();
        }

        void WaveChanged(int oldWave, int newWave)
        {
            waveChangedEvent.Raise(newWave);
        }

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
            var spider = enemyList.GetEnemy(2);
            currentWave++;

            WaveChanged(currentWave, currentWave);

            int waveEnemyCount = (int) (5 * Mathf.Sqrt(Mathf.Pow(currentWave, 3)));
            float spawnDelay = Mathf.Max(5f / currentWave, 0.05f);
            Queue<WaveCommand> commands = new Queue<WaveCommand>();

            yield return new WaitForSeconds(6f);

            switch (currentWave)
            {
                case 7:
                    for (int i = 0; i < (waveEnemyCount / 3); i++)
                    {
                        commands.Enqueue(new SpawnEnemyPrefab(spider, enemyHolder, waypoints[0], waypoints));
                        spawnDelay = 0.25f;
                    }

                    break;
                case 10:
                    commands.Enqueue(new SpawnEnemyPrefab(boss, enemyHolder, waypoints[0], waypoints));
                    break;
                default:
                    for (int i = 0; i < waveEnemyCount; i++)
                    {
                        commands.Enqueue(new SpawnEnemyPrefab(prefab, enemyHolder, waypoints[0], waypoints));
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