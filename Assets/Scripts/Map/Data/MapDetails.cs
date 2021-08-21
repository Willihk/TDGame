using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TDGame.Map.Data
{
    [CreateAssetMenu(fileName = "MapDetails", menuName = "Data/Map/MapDetails", order = 0)]
    public class MapDetails : ScriptableObject
    {
        public string Name;

        public string Description;

        public int2 size;

        [Tooltip("Reference to the scene containing the map")]
        public AssetReference MapReference;
    }
}