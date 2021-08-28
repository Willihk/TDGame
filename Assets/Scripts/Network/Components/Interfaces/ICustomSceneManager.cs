using Cysharp.Threading.Tasks;

namespace TDGame.Network.Components.Interfaces
{
    public interface ICustomSceneManager
    {
        /// <summary>
        /// Loads the specified scene locally.
        /// </summary>
        /// <param name="sceneID"></param>
        /// <returns></returns>
        public UniTask<bool> LoadScene(string sceneID);

        /// <summary>
        /// Unloads the specified scene locally.
        /// </summary>
        /// <param name="sceneID">Asset id of the scene</param>
        /// <returns></returns>
        public UniTask UnloadScene(string sceneID);

        /// <summary>
        ///  Unloads all scenes locally.
        /// </summary>
        /// <returns></returns>
        public UniTask UnLoadAllLoadedScenes();

        /// <summary>
        /// Called on the server.
        /// Loads the specified scene on all clients.
        /// </summary>
        /// <param name="sceneID">AssetID of the scene</param>
        /// <returns></returns>
        public UniTask<bool> LoadSceneSynced(string sceneID);

        /// <summary>
        /// Called on the server.
        /// Unloads the specified scene on all clients.
        /// </summary>
        /// <param name="sceneID">AssetID of the scene</param>
        /// <returns></returns>
        public UniTask<bool> UnloadSceneSynced(string sceneID);

        /// <summary>
        ///  Unloads all scenes on all clients and on the server.
        /// </summary>
        /// <returns></returns>
        public UniTask UnLoadAllLoadedScenesSynced();

    }
}
