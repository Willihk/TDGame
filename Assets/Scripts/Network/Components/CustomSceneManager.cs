using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace TDGame.Network.Components
{
    // TODO: Convert to non MonoBehaviour
    public class CustomSceneManager : MonoBehaviour
    {
        [SerializeField]
        private bool DontDestroyOnLoad = true;
        [SerializeField]
        private AssetReference menuScene;
        
        private List<string> LoadedScenes = new List<string>();

        private void Awake()
        {
            if (DontDestroyOnLoad)
                DontDestroyOnLoad(this);
        }

        private void Start()
        {
            // TODO: Call this from another script at scene load
            LoadScene(menuScene.AssetGUID);
        }

        public void LoadScene(string sceneID)
        {
            var scene = new AssetReference(sceneID);

            if (scene.RuntimeKeyIsValid())
                LoadAddressableScene(scene).Forget();
        }

        private async UniTask LoadAddressableScene(AssetReference scene)
        {
            await scene.LoadSceneAsync(LoadSceneMode.Additive);
            LoadedScenes.Add(scene.AssetGUID);
        }
        
        private async UniTask UnLoadAddressableScene(AssetReference scene)
        {
            await scene.UnLoadScene();
            LoadedScenes.Remove(scene.AssetGUID);
        }
    }
}