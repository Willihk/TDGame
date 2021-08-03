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
using UnityEngine.ResourceManagement.ResourceProviders;
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
        private Dictionary<string, SceneInstance> loadedScenes = new Dictionary<string, SceneInstance>();

        public Dictionary<string, SceneInstance> LoadedScenes => loadedScenes;

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
            if (loadedScenes.ContainsKey(sceneID))
            {
                UnLoadAddressableScene(sceneID).Forget();
                return true;
            }

            return false;
        }

        public bool UnLoadAllLoadedScenes()
        {
            var scenes = loadedScenes.Keys.ToArray();

            foreach (var scene in scenes)
            {
                UnLoadAddressableScene(scene).Forget();
            }

            return true;
        }

        public async UniTask SwitchScenes(string currentSceneID, string newSceneID)
        {
            var newScene = new AssetReference(newSceneID);
            if (loadedScenes.TryGetValue(currentSceneID, out SceneInstance currentScene) &&
                newScene.RuntimeKeyIsValid())
            {
                await UnLoadAddressableScene(currentSceneID);

                await LoadAddressableScene(newScene);
            }
        }

        private async UniTask LoadAddressableScene(AssetReference scene)
        {
            var sceneInstance = await Addressables.LoadSceneAsync(scene, LoadSceneMode.Additive);
            loadedScenes.Add(scene.AssetGUID, sceneInstance);
        }

        private async UniTask UnLoadAddressableScene(string sceneID)
        {
            Addressables.UnloadSceneAsync(loadedScenes[sceneID]);
            loadedScenes.Remove(sceneID);
        }


        public async UniTaskVoid ReturnMainMenu()
        {
            var scenes = loadedScenes.Keys.ToArray();
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
            sender.Send(new LoadedScenes()
            {
                LoadedSceneIDs = loadedScenes.Keys.ToList()
            });
        }

        public void Handle_LoadedScenes(INetworkPlayer sender, LoadedScenes message)
        {
            LoadScenesFromMessage(message).Forget();
        }

        private async UniTask LoadScenesFromMessage(LoadedScenes message)
        {
            List<UniTask> handles = new List<UniTask>();
            var toLoad = message.LoadedSceneIDs.Where(x => !loadedScenes.ContainsKey(x)).ToList();

            var toUnLoad = loadedScenes.Where(x => !message.LoadedSceneIDs.Contains(x.Key)).Select(x => x.Key)
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