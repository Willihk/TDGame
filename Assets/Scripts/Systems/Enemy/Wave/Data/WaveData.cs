using System.Collections.Generic;
using UnityEngine;

namespace TDGame.Systems.Enemy.Wave.Data
{
    [CreateAssetMenu(fileName = "Wave", menuName = "Data/Wave/WaveData", order = 0)]
    public class WaveData : ScriptableObject
    {
        public List<WaveAction> Actions;
    }
}