using System.Collections.Generic;
using TDGame.Systems.Stats;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TDGame.Systems.Enemy.Data
{
    [CreateAssetMenu(fileName = "EnemyDetails", menuName = "Data/Enemy/EnemyDetails", order = 0)]
    public class EnemyDetails : ScriptableObject
    {
        public string displayName;

        public string description;

        
        public List<Stat> stats;


        [Tooltip("Reference to the prefab")]
        public AssetReference assetReference;
    }
}