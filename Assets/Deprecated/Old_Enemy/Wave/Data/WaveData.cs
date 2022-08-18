using System.Collections.Generic;
using UnityEngine;

namespace TDGame.Systems.Old_Enemy.Wave.Data
{
    [CreateAssetMenu(fileName = "Wave", menuName = "Data/Wave/WaveData", order = 0)]
    public class WaveData : ScriptableObject
    {
        public int Level;
        public List<WaveAction> Actions;
    }
}