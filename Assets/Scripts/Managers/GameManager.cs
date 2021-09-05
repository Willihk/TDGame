using TDGame.Network.Components;
using TDGame.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TDGame.Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private CustomSceneManager sceneManager;

        [SerializeField]
        AssetReference gameplayScene;

        [SerializeField]
        LobbySettings lobbySettings;

        public async void LobbyStartGame()
        {
            Debug.Log("lobby start game");

            await sceneManager.UnLoadAllLoadedScenesSynced();

            await sceneManager.LoadSceneSynced(gameplayScene.AssetGUID);


            // Unload lobby scene
            // Load gameplay scene
            // Load map scene
            // Call setup event for gameplay
        }
    }
}