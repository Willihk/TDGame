using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using Cysharp.Threading.Tasks;
using Mirage;
using Sirenix.OdinInspector;
using TDGame.Network.Messages.Scene;
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
        [Sirenix.OdinInspector.ShowInInspector]
        private Dictionary<string, AssetReference> loadedScenes = new Dictionary<string, AssetReference>();

        public Dictionary<string, AssetReference> LoadedScenes => loadedScenes;

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

        public bool UnLoadAllLoadedScenes()
        {
            var scenes = loadedScenes.Values.ToArray();

            foreach (var scene in scenes)
            {
                UnLoadAddressableScene(scene).Forget();
            }

            return true;
        }

        public bool SwitchScenes(string currentSceneID, string newSceneID)
        {
            var newScene = new AssetReference(newSceneID);
            if (loadedScenes.TryGetValue(currentSceneID, out AssetReference currentScene) &&
                newScene.RuntimeKeyIsValid())
            {
                currentScene.UnLoadScene().Completed += handle =>
                {
                    loadedScenes.Remove(currentScene.AssetGUID);

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


        public async UniTaskVoid ReturnMainMenu()
        {
            var scenes = loadedScenes.Values.ToArray();

            foreach (var scene in scenes)
            {
                await UnLoadAddressableScene(scene);
            }

            await LoadAddressableScene(menuScene);
        }

        #region Scene syncing

        public void Server_OnClientConnected(INetworkPlayer player)
        {
            player.RegisterHandler<RequestLoadedScenes>(Handle_RequestLoadedScenes);
        }

        public void Client_OnConnected(INetworkPlayer server)
        {
            server.RegisterHandler<LoadedScenes>(Handle_LoadedScenes);
            server.Send(new RequestLoadedScenes());
        }

        public void Handle_RequestLoadedScenes(INetworkPlayer sender, RequestLoadedScenes message)
        {
            sender.Send(new LoadedScenes() { LoadedSceneIDs = loadedScenes.Keys.ToList() });
        }

        public void Handle_LoadedScenes(INetworkPlayer sender, LoadedScenes message)
        {
            LoadScenesFromMessage(message).Forget();
        }

        private async UniTask LoadScenesFromMessage(LoadedScenes message)
        {
            List<UniTask> handles = new List<UniTask>();

            var toLoad = message.LoadedSceneIDs.Where(x => !loadedScenes.ContainsKey(x)).ToList();
            var toUnLoad = loadedScenes.Where(x => !message.LoadedSceneIDs.Contains(x.Key)).Select(x => x.Value)
                .ToList();

            toLoad.ForEach(x =>
            {
                // Load scene
                AssetReference scene = new AssetReference(x);
                if (!scene.RuntimeKeyIsValid())
                    Debug.LogError("Invalid scene AssetReference");

                handles.Add(LoadAddressableScene(scene));
            });

            toUnLoad.ForEach(x => handles.Add(UnLoadAddressableScene(x)));

            await UniTask.WhenAll(handles);
        }

        #endregion
    }
}