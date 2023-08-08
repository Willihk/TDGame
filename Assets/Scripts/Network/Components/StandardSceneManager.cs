using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using MessagePack;
using Sirenix.OdinInspector;
using TDGame.Network.Components.Interfaces;
using TDGame.Network.Components.Messaging;
using TDGame.Network.Messages.Scene;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TDGame.Network.Components
{
    // TODO: Convert to non MonoBehaviour
    public class StandardSceneManager : MonoBehaviour, ICustomSceneManager
    {
        public static StandardSceneManager Instance;

        [ReadOnly]
        [ShowInInspector]
        private List<string> loadedScenes = new();

        public List<string> LoadedScenes => loadedScenes;

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
            await LoadAddressableScene(sceneID);

            return true;
        }

        public async UniTask UnloadScene(string sceneID)
        {
            if (loadedScenes.Contains(sceneID))
            {
                await UnLoadAddressableScene(sceneID);
            }
        }

        public async UniTask UnLoadAllLoadedScenes()
        {
            var scenes = loadedScenes.ToArray(); // copy to new array, scenes are removed from loadedScenes when running.
            foreach (string scene in scenes)
            {
                await UnLoadAddressableScene(scene);
            }
        }

        private async UniTask LoadAddressableScene(string scene)
        {
            if (loadedScenes.Contains(scene))
            {
                Debug.LogWarning("Scene is already loaded!");
                return;
            }

            loadedScenes.Add(scene);

            await SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            Debug.Log("Loaded scene: "+scene);

        }

        private async UniTask UnLoadAddressableScene(string sceneID)
        {
            loadedScenes.Remove(sceneID);
            await SceneManager.UnloadSceneAsync(sceneID);
            Debug.Log("Unloaded scene: "+sceneID);
        }

        #region Scene syncing

        public void Client_OnConnected()
        {
            messagingManager.SendNamedMessageToServer(new RequestLoadedScenes());
        }

        private void Handle_RequestLoadedScenes(TDNetworkConnection sender, Stream stream)
        {
            var response = new SyncLoadedScenes()
            {
                LoadedSceneIDs = loadedScenes
            };

            messagingManager.SendNamedMessage(sender, response);
        }

        private void Handle_SyncLoadedScenes(TDNetworkConnection sender, Stream stream)
        {
            var message = MessagePackSerializer.Deserialize<SyncLoadedScenes>(stream);

            LoadScenesFromMessage(message).Forget();
        }

        private async UniTask LoadScenesFromMessage(SyncLoadedScenes message)
        {
            var handles = new List<UniTask>();
            var toLoad = message.LoadedSceneIDs.Where(x => !loadedScenes.Contains(x)).ToList();

            var toUnLoad = loadedScenes.Where(x => !message.LoadedSceneIDs.Contains(x))
                .ToList();

            toLoad.ForEach(x => handles.Add(LoadAddressableScene(x)));
            toUnLoad.ForEach(x => handles.Add(UnLoadAddressableScene(x)));
            await UniTask.WhenAll(handles);
        }

        public async UniTask<bool> LoadSceneSynced(string sceneID)
        {
            if (CustomNetworkManager.Instance.serverWrapper.isListening &&
                !CustomNetworkManager.Instance.clientWrapper.isConnected)
            {
                await LoadScene(sceneID);
            }

            var message = new LoadSceneMessage { SceneID = sceneID };

            messagingManager.SendNamedMessageToAll(message);

            return true;
        }

        public async UniTask<bool> UnloadSceneSynced(string sceneID)
        {
            if (CustomNetworkManager.Instance.serverWrapper.isListening &&
                !CustomNetworkManager.Instance.clientWrapper.isConnected)
            {
                await UnloadScene(sceneID);
            }

            var message = new UnloadSceneMessage { SceneID = sceneID };

            messagingManager.SendNamedMessageToAll(message);

            return true;
        }

        public async UniTask UnLoadAllLoadedScenesSynced()
        {
            var scenes = loadedScenes.ToArray(); // copy to new array, scenes are removed from loadedScenes when running.
            foreach (string scene in scenes)
            {
                await UnloadSceneSynced(scene);
            }
        }

        void Handle_LoadScene(TDNetworkConnection sender, Stream stream)
        {
            var message = MessagePackSerializer.Deserialize<LoadSceneMessage>(stream);
            LoadScene(message.SceneID).Forget();
        }

        void Handle_UnloadScene(TDNetworkConnection sender, Stream stream)
        {
            var message = MessagePackSerializer.Deserialize<UnloadSceneMessage>(stream);
            UnloadScene(message.SceneID).Forget();
        }

        #endregion
    }
}