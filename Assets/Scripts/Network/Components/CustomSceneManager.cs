using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using MessagePack;
using MLAPI;
using MLAPI.Messaging;
using Sirenix.OdinInspector;
using TDGame.Network.Components.Interfaces;
using TDGame.Network.Messages.Scene;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace TDGame.Network.Components
{
    // TODO: Convert to non MonoBehaviour
    public class CustomSceneManager : MonoBehaviour, ICustomSceneManager
    {
        [ReadOnly]
        [Sirenix.OdinInspector.ShowInInspector]
        private Dictionary<string, SceneInstance> loadedScenes = new Dictionary<string, SceneInstance>();

        public Dictionary<string, SceneInstance> LoadedScenes => loadedScenes;

        //[SerializeField]
        //private networkmanager networkServer;
        [SerializeField]
        private NetworkManager networkManager;

        void awake()
        {
            TDGameMessagingManager.RegisterNamedMessageHandler<SyncLoadedScenes>(Handle_SyncLoadedScenes);
            TDGameMessagingManager.RegisterNamedMessageHandler<LoadScene>(Handle_LoadScene);
            TDGameMessagingManager.RegisterNamedMessageHandler<UnloadScene>(Handle_UnloadScene);
            TDGameMessagingManager.RegisterNamedMessageHandler<RequestLoadedScenes>(Handle_RequestLoadedScenes);
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

        public void Client_OnConnected(ulong id)
        {
            TDGameMessagingManager.SendNamedMessage(id, new RequestLoadedScenes());
        }

        private void Handle_RequestLoadedScenes(ulong sender, Stream stream)
        {
            var respone = new SyncLoadedScenes()
            {
                LoadedSceneIDs = loadedScenes.Keys.ToList()
            };

            MemoryStream data = new MemoryStream(MessagePackSerializer.Serialize(respone));
            CustomMessagingManager.SendNamedMessage(nameof(SyncLoadedScenes), sender, data);
        }

        private void Handle_SyncLoadedScenes(ulong sender, Stream stream)
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
            if (networkManager.IsServer && !networkManager.IsHost)
            {

                await LoadScene(sceneID);
            }

            var message = new UnloadScene { SceneID = sceneID };


            MemoryStream data = new MemoryStream(MessagePackSerializer.Serialize(message));
            TDGameMessagingManager.SendNamedMessageToAll(nameof(UnloadScene), data);

            return true;
        }

        public async UniTask<bool> UnloadSceneSynced(string sceneID)
        {
            if (networkManager.IsServer && !networkManager.IsHost)
            {
                await UnloadScene(sceneID);
            }

            var message = new UnloadScene { SceneID = sceneID };

            
            MemoryStream data = new MemoryStream(MessagePackSerializer.Serialize(message));
            TDGameMessagingManager.SendNamedMessageToAll(nameof(UnloadScene), data);

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

        void Handle_LoadScene(ulong sender, Stream stream)
        {
            var message = MessagePackSerializer.Deserialize<LoadScene>(stream);
            LoadScene(message.SceneID).Forget();
        }

        void Handle_UnloadScene(ulong sender, Stream stream)
        {
            var message = MessagePackSerializer.Deserialize<UnloadScene>(stream);
            UnloadScene(message.SceneID).Forget();
        }

        #endregion
    }
}