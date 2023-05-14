using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using MessagePack;
using TDGame.Events;
using TDGame.Network.Components;
using TDGame.Network.Components.Messaging;
using TDGame.PrefabManagement;
using TDGame.Systems.Wave.Messages;
using Unity.Entities;
using UnityEngine;
using Hash128 = Unity.Entities.Hash128;

namespace TDGame.Systems.Wave
{
    public enum WaveState
    {
        Idle,
        Running
    }

    public struct WaveGlobalState : IComponentData
    {
        public WaveState State;
        public float WaveElapsedTime;
    }

    [MessagePackObject]
    public struct WaveSpawnEntry : IComponentData
    {
        [Key(0)]
        public Hash128 Guid;

        [Key(1)]
        public float Time;
    }

    [MessagePackObject]
    public struct WaveData
    {
        [Key(0)]
        public int WaveNumber;

        [Key(1)]
        public WaveSpawnEntry[] SpawnEntries;
    }

    public class WaveManager : MonoBehaviour
    {
        public static WaveManager Instance;

        public int currentWave = 0;

        private Dictionary<int, WaveData> storedWaves = new();

        private EntityManager entityManager;

        private BaseMessagingManager messagingManager;

        private Entity waveStateSingleton;


        private void Start()
        {
            Instance = this;

            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            waveStateSingleton = entityManager.CreateSingleton(new WaveGlobalState
            {
                State = WaveState.Idle,
                WaveElapsedTime = 0
            }, "WaveState");

            messagingManager = BaseMessagingManager.Instance;

            EventManager.Instance.onClickNextWave.EventListeners += StartNextWave;

            messagingManager.RegisterNamedMessageHandler<ResetWaveMessage>(ResetWaveHandler);
            messagingManager.RegisterNamedMessageHandler<StoreWaveMessage>(StoreWaveHandler);
            messagingManager.RegisterNamedMessageHandler<GenerateAndStoreWaveMessage>(GenerateAndStoreWaveHandler);
            messagingManager.RegisterNamedMessageHandler<LoadWaveMessage>(LoadWaveHandler);
            messagingManager.RegisterNamedMessageHandler<StartWaveMessage>(StartWaveHandler);
        }

        private void OnDestroy()
        {
            EventManager.Instance.onClickNextWave.EventListeners -= StartNextWave;
        }

        public static WaveData GenerateWave(int waveNumber)
        {
            var wave = new WaveData
            {
                WaveNumber = waveNumber
            };

            uint spawnCount = 20 * (uint)Math.Pow(wave.WaveNumber, 1.2);

            wave.SpawnEntries = new WaveSpawnEntry[spawnCount];

            float spread = 20f / spawnCount;

            var prefab = PrefabManager.Instance.prefabList.GetGameObject(0);
            var hash = PrefabManager.Instance.GetPrefabHash(prefab.name);

            for (int i = 0; i < wave.SpawnEntries.Length; i++)
            {
                wave.SpawnEntries[i].Guid = hash;
                wave.SpawnEntries[i].Time = i * spread;
            }

            return wave;
        }


        public async void StartNextWave()
        {
            if (!CustomNetworkManager.Instance.serverWrapper.isListening)
                return;

            // Reset state
            messagingManager.SendNamedMessageToAll(new ResetWaveMessage());

            // prepare wave
            // var wave = GenerateWave();

            int newWave = currentWave + 1;
            messagingManager.SendNamedMessageToAll(new GenerateAndStoreWaveMessage { WaveNumber = newWave});

            await UniTask.Delay(500);
            messagingManager.SendNamedMessageToAll(new LoadWaveMessage() { WaveNumber =  newWave });


            await UniTask.Delay(500); // give time for other players to load wave.

            // start

            messagingManager.SendNamedMessageToAll(new StartWaveMessage());
        }

        public void WaveCompleted()
        {
            EventManager.Instance.onWaveCompleted.Raise(currentWave);
        }


        private void ResetWaveHandler(TDNetworkConnection sender, Stream data)
        {
            entityManager.SetComponentData(waveStateSingleton,
                new WaveGlobalState { State = WaveState.Idle, WaveElapsedTime = 0 });
        }

        private void StoreWaveHandler(TDNetworkConnection sender, Stream data)
        {
            var message = MessagePackSerializer.Deserialize<StoreWaveMessage>(data);

            storedWaves.Add(message.Data.WaveNumber, message.Data);
        }

        private void LoadWaveHandler(TDNetworkConnection sender, Stream data)
        {
            var message = MessagePackSerializer.Deserialize<LoadWaveMessage>(data);

            var wave = storedWaves[message.WaveNumber];

            foreach (var entry in wave.SpawnEntries)
            {
                var entity = entityManager.CreateEntity();
                entityManager.AddComponent<WaveSpawnEntry>(entity);
                entityManager.SetComponentData(entity, entry);
            }

            storedWaves.Remove(message.WaveNumber);

            currentWave = wave.WaveNumber;
        }


        private void GenerateAndStoreWaveHandler(TDNetworkConnection sender, Stream data)
        {
            var message = MessagePackSerializer.Deserialize<GenerateAndStoreWaveMessage>(data);

            var wave = GenerateWave(message.WaveNumber);
            storedWaves.Add(message.WaveNumber, wave);
        }

        private void StartWaveHandler(TDNetworkConnection sender, Stream data)
        {
            entityManager.SetComponentData(waveStateSingleton,
                new WaveGlobalState { State = WaveState.Running, WaveElapsedTime = 0 });
            EventManager.Instance.onWaveStarted.Raise(currentWave);
        }
    }
}