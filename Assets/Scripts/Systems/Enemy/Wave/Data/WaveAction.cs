using System;
using UnityEngine;

namespace TDGame.Systems.Enemy.Wave.Data
{
    [Serializable]
    public struct WaveAction
    {
        public WaveActionType ActionType;
        public GameObject Prefab;

        public float Delay;
    }

    public enum WaveActionType
    {
        SpawnPrefab,
        SetDelay
    }
}