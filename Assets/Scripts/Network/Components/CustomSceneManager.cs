using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using MessagePack;
using Mirror;
using Sirenix.OdinInspector;
using TDGame.Network.Components.Interfaces;
using TDGame.Network.Components.Messaging;
using TDGame.Network.Messages.Scene;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using NetworkConnection = TDGame.Network.Components.Messaging.NetworkConnection;

namespace TDGame.Network.Components
{
    // TODO: Convert to non MonoBehaviour
    public class CustomSceneManager : MonoBehaviour, ICustomSceneManager
    {

        public static CustomSceneManager Instance;
        
        [ReadOnly]
        [Sirenix.OdinInspector.ShowInInspector]
        private Dictionary<string, SceneInstance> loadedScenes = new Dictionary<string, SceneInstance>();

        public Dictionary<string, SceneInstance> LoadedScenes => loadedScenes;

        BaseMessagingManager messagingManager;

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            messagingManager = BaseMessagingManager.Instance;

            messagingManager.RegisterNamedMessageHandler<SyncLoadedScenes>(Handle_SyncLoadedScenes);
            messagingManager.RegisterNamedMessageHandler<LoadSceneMessage>(Handle_LoadScene);
            messagingManager.RegisterNamedMessageHandler<UnloadSceneMessage>(Handle_UnloadScene);
            messagingManager.RegisterNamedMessageHandler<RequestLoadedScenes>(Handle_RequestLoadedScenes);
        }

        public async UniTask<bool> LoadScene(string sceneID)
        {
            var scene = new AssetReference(sceneID);

            if (scene.RuntimeKeyIsValid())
                await LoadAddressableScene(scene);

            return scene.RuntimeKeyIsValid();
        }

        public async UniTask UnloadScene(string sceneID)
        {
            if (loadedScenes.ContainsKey(sceneID))
            {
                await UnLoadAddressableScene(sceneID);
            }
        }

        public async UniTask UnLoadAllLoadedScenes()
        {
            var scenes = loadedScenes.Keys.ToArray();

            foreach (var scene in scenes)
            {
                await UnLoadAddressableScene(scene);
            }
        }

        private async UniTask LoadAddressableScene(AssetReference scene)
        {
            if (loadedScenes.ContainsKey(scene.AssetGUID))
                return;

            loadedScenes.Add(scene.AssetGUID, new SceneInstance());

            var sceneInstance = await Addressables.LoadSceneAsync(scene, LoadSceneMode.Additive);
            loadedScenes[scene.AssetGUID] = sceneInstance;
        }

        private async UniTask UnLoadAddressableScene(string sceneID)
        {
            var instance = loadedScenes[sceneID];
            loadedScenes.Remove(sceneID);

            await Addressables.UnloadSceneAsync(instance);
        }

        #region Scene syncing

        public void Client_OnConnected()
        {
            messagingManager.SendNamedMessageToServer(new RequestLoadedScenes());
        }

        private void Handle_RequestLoadedScenes(NetworkConnection sender, Stream stream)
        {
            var response = new SyncLoadedScenes()
            {
                LoadedSceneIDs = loadedScenes.Keys.ToList()
            };

            messagingManager.SendNamedMessage(sender, response);
        }

        private void Handle_SyncLoadedScenes(NetworkConnection sender, Stream stream)
        {
            var message = MessagePackSerializer.Deserialize<SyncLoadedScenes>(stream);

            LoadScenesFromMessage(message).Forget();
        }

        private async UniTask LoadScenesFromMessage(SyncLoadedScenes message)
        {
            var handles = new List<UniTask>();
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

        public async UniTask<bool> LoadSceneSynced(string sceneID)
        {
            if (NetworkServer.active && !NetworkClient.active)
            {

                await LoadScene(sceneID);
            }

            var message = new LoadSceneMessage { SceneID = sceneID };

            messagingManager.SendNamedMessageToAll(message);

            return true;
        }

        public async UniTask<bool> UnloadSceneSynced(string sceneID)
        {
            if (NetworkServer.active && !NetworkClient.active)
            {
                await UnloadScene(sceneID);
            }

            var message = new UnloadSceneMessage { SceneID = sceneID };

            messagingManager.SendNamedMessageToAll(message);

            return true;
        }

        public async UniTask UnLoadAllLoadedScenesSynced()
        {
            // TODO: Create message to unload all scenes.
            var scenes = loadedScenes.Keys.ToArray();

            foreach (var scene in scenes)
            {
                await UnloadSceneSynced(scene);
            }
        }

        void Handle_LoadScene(NetworkConnection sender, Stream stream)
        {
            var message = MessagePackSerializer.Deserialize<LoadSceneMessage>(stream);
            LoadScene(message.SceneID).Forget();
        }

        void Handle_UnloadScene(NetworkConnection sender, Stream stream)
        {
            var message = MessagePackSerializer.Deserialize<UnloadSceneMessage>(stream);
            UnloadScene(message.SceneID).Forget();
        }

        #endregion
    }
}