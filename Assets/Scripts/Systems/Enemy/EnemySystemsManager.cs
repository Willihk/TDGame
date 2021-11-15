using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TDGame.Systems.Enemy
{
    public class EnemySystemsManager : MonoBehaviour
    {
        private async void Start()
        {
            var keys = await Addressables.LoadResourceLocationsAsync("ExampleEnemy.prefab"); // Handle might need to be released -who knows???
            
        }
    }
}