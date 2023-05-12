using System;
using UnityEngine;
using Hash128 = Unity.Entities.Hash128;

namespace TDGame.Systems.Wave
{

    public struct WaveSpawnEntry
    {
        public Hash128 Guid;
        public float Time;
    }
    
    public struct WaveData
    {
        public WaveSpawnEntry[] SpawnEntries;
    }
    
    public class WaveManager : MonoBehaviour
    {
        public static WaveManager Instance;

        public int currentWave = 0;

        private void Start()
        {
            Instance = this;
        }


        public WaveData GenerateWave()
        {
            return new WaveData();
        }
        
    }
}