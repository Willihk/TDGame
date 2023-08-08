using Cysharp.Threading.Tasks;
using TDGame.Events;
using TDGame.Network.Components;
using TDGame.Settings;
using Unity.Entities;
using Unity.Scenes;
using UnityEngine;

namespace TDGame.Managers
{
    public class GameManager : MonoBehaviour
    {
        public int PlayerHealth = 100;

        
        [SerializeField]
        private StandardSceneManager sceneManager;

        [SerializeField]
        string gameplayScene;

        [SerializeField]
        LobbySettings lobbySettings;

        private EntityManager entityManager;

        private void Start()
        {
            EventManager.Instance.onClickStartGame.EventListeners += LobbyStartGame;
            EventManager.Instance.onEnemyReachedEnd.EventListeners += EnemyReachedEnd;
        }

        public async void LobbyStartGame()
        {
            Debug.Log("lobby start game");

            // Unload lobby scene
            await sceneManager.UnLoadAllLoadedScenesSynced();

            // Load gameplay scene
            await sceneManager.LoadSceneSynced(gameplayScene);

            // Load map scene
            await sceneManager.LoadSceneSynced(lobbySettings.selectedMap.MapReference);

            // Setup for gameplay

            PlayerHealth = 100;
            EventManager.Instance.onPlayerHealthChanged.Raise(PlayerHealth);
            
            EventManager.Instance.onPathRegistered.EventListeners += StartGame;
            await UniTask.Delay(5000);
            
            EventManager.Instance.onMapLoaded.Raise();
        }

        void StartGame()
        {
            EventManager.Instance.onPathRegistered.EventListeners -= StartGame;
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            
            entityManager.CreateSingleton(new GameData {State = GameState.Playing}, "GameState");
        }

        public void EnemyReachedEnd()
        {
            PlayerHealth--;
            EventManager.Instance.onPlayerHealthChanged.Raise(PlayerHealth);
        }
    }
}