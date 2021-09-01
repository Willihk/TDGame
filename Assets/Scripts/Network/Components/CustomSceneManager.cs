using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.Serialization.Pooled;
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

        private NetworkManager networkManager;

        void awake()
        {
            CustomMessagingManager.RegisterNamedMessageHandler(nameof(SyncLoadedScenes), Handle_SyncLoadedScenes);

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

        public void Server_OnClientConnected(INetworkPlayer player)
        {
            player.RegisterHandler<RequestLoadedScenes>(Handle_RequestLoadedScenes);
        }

        public void Client_OnConnected(INetworkPlayer server)
        {
            server.RegisterHandler<SyncLoadedScenes>(Handle_SyncLoadedScenes);
            server.RegisterHandler<LoadScene>(Handle_LoadScene);
            server.RegisterHandler<UnloadScene>(Handle_UnloadScene);

            server.Send(new RequestLoadedScenes());
        }

        private void Handle_RequestLoadedScenes(ulong sender, RequestLoadedScenes message)
        {
            CustomMessagingManager.SendNamedMessage(nameof(SyncLoadedScenes), sender, )
            sender.Send(new SyncLoadedScenes()
            {
                LoadedSceneIDs = loadedScenes.Keys.ToList()
            });
        }

        private void Handle_SyncLoadedScenes(ulong sender, Stream stream)
        {
            using (PooledNetworkReader reader = PooledNetworkReader.Get(stream))
            {
                var message = (SyncLoadedScenes)reader.ReadObjectPacked(typeof(SyncLoadedScenes));
                LoadScenesFromMessage(message).Forget();
            }
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
            if (!networkServer.LocalClientActive)
            {
                await LoadScene(sceneID);
            }

            networkServer.SendToAll(new LoadScene { SceneID = sceneID });

            return true;
        }

        public async UniTask<bool> UnloadSceneSynced(string sceneID)
        {
            if (!networkServer.LocalClientActive)
            {
                await UnloadScene(sceneID);
            }
            networkServer.SendToAll(new UnloadScene { SceneID = sceneID });

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

        void Handle_LoadScene(INetworkPlayer sender, LoadScene message)
        {
            LoadScene(message.SceneID).Forget();
        }

        void Handle_UnloadScene(INetworkPlayer sender, UnloadScene message)
        {
            UnloadScene(message.SceneID).Forget();
        }

        #endregion
    }
}