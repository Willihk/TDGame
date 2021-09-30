using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TDGame.Systems.Tower.Data
{
    [CreateAssetMenu(fileName = "TowerDetails", menuName = "Data/Tower/TowerDetails", order = 0)]
    public class TowerDetails : ScriptableObject
    {
        public string Name;

        public string Description;

        public int Price;


        [Tooltip("Reference to the tower prefab")]
        public AssetReference TowerReference;
    }
}