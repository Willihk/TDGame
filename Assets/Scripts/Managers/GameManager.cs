using TDGame.Network.Components;
using TDGame.Settings;
using UnityEngine;

namespace TDGame.Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private CustomSceneManager sceneManager;

        [SerializeField]
        LobbySettings lobbySettings;

        public void LobbyStartGame()
        {
            Debug.Log("lobby start game");

            sceneManager.LoadSceneSynced(lobbySettings.selectedMap.MapReference.AssetGUID);
            // Unload lobby scene
            // Load gameplay scene
            // Load map scene
            // Call setup event for gameplay
        }
    }
}