using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using TDGame.Command.Implementations.Wave;
using TDGame.Events.Base;
using TDGame.Map;
using TDGame.Systems.Enemy.Data;
using TDGame.Systems.Enemy.Manager;
using TDGame.Systems.Enemy.Wave.Data;
using UnityEngine;

namespace TDGame.Systems.Enemy.Wave
{
    public class EnemyWaveController : NetworkBehaviour
    {
        [SerializeField]
        private List<WaveData> predefinedWaves;

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

        private float delay;

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

            if (EnemyManager.Instance.targets.Count == 0 && AwaitingNextWave)
            {
                Queue<WaveCommand> commands;
                if (predefinedWaves?.Count > 99999)
                {
                    var wave = predefinedWaves[0];
                    commands = LoadWave(wave);
                }
                else
                {
                    commands = CreateTestWave();
                }

                AwaitingNextWave = false;
                StartCoroutine(nameof(SpawnWave), commands);
            }
        }


        Queue<WaveCommand> LoadWave(WaveData waveData)
        {
            Queue<WaveCommand> commands = new Queue<WaveCommand>();

            foreach (var action in waveData.Actions)
            {
                switch (action.ActionType)
                {
                    case WaveActionType.SpawnPrefab:
                        commands.Enqueue(new SpawnEnemyPrefab(action.Prefab, transform, waypoints[0], waypoints));
                        break;
                    case WaveActionType.SetDelay:
                        throw new NotImplementedException();
                }
            }

            return commands;
        }

        Queue<WaveCommand> CreateTestWave()
        {
            var prefab = enemyList.GetEnemy(0);
            var boss = enemyList.GetEnemy(1);
            var spider = enemyList.GetEnemy(2);


            int waveEnemyCount = (int) (5 * Mathf.Sqrt(Mathf.Pow(currentWave, 3)));
            Queue<WaveCommand> commands = new Queue<WaveCommand>();

            switch (currentWave)
            {
                case 7:
                    for (int i = 0; i < (waveEnemyCount / 3); i++)
                    {
                        commands.Enqueue(new SpawnEnemyPrefab(spider, enemyHolder, waypoints[0], waypoints));
                    }

                    break;
                case 10:
                    commands.Enqueue(new SpawnEnemyPrefab(boss, enemyHolder, waypoints[0], waypoints));
                    break;
                default:
                    for (int i = 0; i < waveEnemyCount; i++)
                    {
                        commands.Enqueue(new SpawnEnemyPrefab(prefab, enemyHolder, waypoints[0], waypoints));
                        commands.Enqueue(new DelayCommand(this, 5));
                    }

                    break;
            }

            return commands;
        }

        IEnumerator SpawnWave(Queue<WaveCommand> commands)
        {
            currentWave++;

            WaveChanged(currentWave, currentWave);

            float spawnDelay = Mathf.Max(5f / currentWave, 0.05f);
            while (commands.Count > 0)
            {
                commands.Dequeue().Execute();
                yield return new WaitForSeconds(delay);
                delay = 0;
            }

            yield return new WaitForSeconds(1f);

            AwaitingNextWave = true;
        }
    }
}