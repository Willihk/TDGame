using System.Collections.Generic;
using System.Linq;
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
        [ReadOnly]
        [Sirenix.OdinInspector.ShowInInspector]
        private Dictionary<string, SceneInstance> loadedScenes = new Dictionary<string, SceneInstance>();

        public Dictionary<string, SceneInstance> LoadedScenes => loadedScenes;

        public async UniTask<bool> LoadScene(string sceneID)
        {
            var scene = new AssetReference(sceneID);

            if (scene.RuntimeKeyIsValid())
                await LoadAddressableScene(scene);

            return scene.RuntimeKeyIsValid();
        }

        public async UniTask UnLoadScene(string sceneID)
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
            server.RegisterHandler<LoadedScenes>(Handle_LoadedScenes);
            server.Send(new RequestLoadedScenes());
        }

        private void Handle_RequestLoadedScenes(INetworkPlayer sender, RequestLoadedScenes message)
        {
            sender.Send(new LoadedScenes()
            {
                LoadedSceneIDs = loadedScenes.Keys.ToList()
            });
        }

        private void Handle_LoadedScenes(INetworkPlayer sender, LoadedScenes message)
        {
            LoadScenesFromMessage(message).Forget();
        }

        private async UniTask LoadScenesFromMessage(LoadedScenes message)
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

        #endregion
    }
}