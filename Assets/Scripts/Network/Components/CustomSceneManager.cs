using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
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

        [ReadOnly]
        [ShowInInspector]
        private Dictionary<string, AssetReference> loadedScenes = new Dictionary<string, AssetReference>();

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

        public bool LoadScene(string sceneID)
        {
            var scene = new AssetReference(sceneID);

            if (scene.RuntimeKeyIsValid())
                LoadAddressableScene(scene).Forget();

            return scene.RuntimeKeyIsValid();
        }

        public bool UnLoadScene(string sceneID)
        {
            if (loadedScenes.TryGetValue(sceneID, out AssetReference scene))
            {
                UnLoadAddressableScene(scene).Forget();
                return true;
            }

            return false;
        }

        public bool SwitchScenes(string currentSceneID, string newSceneID)
        {
            var newScene = new AssetReference(newSceneID);
            if (loadedScenes.TryGetValue(currentSceneID, out AssetReference currentScene) && newScene.RuntimeKeyIsValid())
            {
                currentScene.UnLoadScene().Completed += handle =>
                {
                    newScene.LoadSceneAsync(LoadSceneMode.Additive); 
                    loadedScenes.Add(newSceneID, newScene);
                };
                return true;
            }

            return false;
        }

        private async UniTask LoadAddressableScene(AssetReference scene)
        {
            await scene.LoadSceneAsync(LoadSceneMode.Additive);
            loadedScenes.Add(scene.AssetGUID, scene);
        }

        private async UniTask UnLoadAddressableScene(AssetReference scene)
        {
            await scene.UnLoadScene();
            loadedScenes.Remove(scene.AssetGUID);
        }
    }
}