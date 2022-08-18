﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using TDGame.Command.Implementations.Wave;
using TDGame.Data;
using TDGame.Events.Base;
using TDGame.Map;
using TDGame.Systems.Grid.InGame;
using TDGame.Systems.Old_Enemy.Manager;
using TDGame.Systems.Old_Enemy.Wave.Data;
using UnityEngine;

namespace TDGame.Systems.Old_Enemy.Wave
{
    public class EnemyWaveController : NetworkBehaviour
    {
        [SerializeField]
        private List<WaveData> predefinedWaves;

        [SerializeField]
        private GameEvent<int> waveChangedEvent;

        [SerializeField]
        private GameObjectList enemyList;

        [SerializeField]
        private Transform enemyHolder;

        [SerializeField]
        private NetworkedMapController mapController;

        [SerializeField]
        private List<Vector3> waypoints;

        [SyncVar(hook = nameof(WaveChanged))]
        private int currentWave = 0;

        private bool AwaitingNextWave = true;

        public float delay;

        public void OnMapLoaded()
        {
            if (!isServer)
                return;

            waypoints = mapController.GetWaypoints().Select(x => Old_GridController.Instance.mapGrid.GridToWorldPosition(x.x, x.y)).ToList();
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
                currentWave++;
                Queue<WaveCommand> commands;
                if (predefinedWaves.Find(x => x.Level == currentWave))
                {
                    var wave = predefinedWaves.Find(x => x.Level == currentWave);
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
                        commands.Enqueue(new DelayCommand(this, action.Delay));
                        break;
                }
            }

            return commands;
        }

        Queue<WaveCommand> CreateTestWave()
        {
            var prefab = enemyList.GetGameObject(0);
            var boss = enemyList.GetGameObject(1);
            var spider = enemyList.GetGameObject(2);


            int waveEnemyCount = (int) (5 * Mathf.Sqrt(Mathf.Pow(currentWave, 3)));
            Queue<WaveCommand> commands = new Queue<WaveCommand>();

            switch (currentWave)
            {
                case 13:
                    for (int i = 0; i < (200); i++)
                    {
                        commands.Enqueue(new SpawnEnemyPrefab(spider, enemyHolder, waypoints[0], waypoints));
                        commands.Enqueue(new DelayCommand(this, 0.1f));
                    }

                    break;
                default:
                    for (int i = 0; i < waveEnemyCount; i++)
                    {
                        commands.Enqueue(new SpawnEnemyPrefab(prefab, enemyHolder, waypoints[0], waypoints));
                        commands.Enqueue(new DelayCommand(this, 0.1f));
                    }

                    break;
            }

            return commands;
        }

        IEnumerator SpawnWave(Queue<WaveCommand> commands)
        {
            WaveChanged(currentWave, currentWave);

            //float spawnDelay = Mathf.Max(5f / currentWave, 0.05f);
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