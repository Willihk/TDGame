using System;
using Cysharp.Threading.Tasks;
using TDGame.Events;
using TDGame.Network.Components;
using TDGame.Settings;
using TDGame.Systems.Grid.InGame;
using Unity.Entities;
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

        private EntityManager entityManager;

        public async void LobbyStartGame()
        {
            Debug.Log("lobby start game");

            // Unload lobby scene
            await sceneManager.UnLoadAllLoadedScenesSynced();

            // Load gameplay scene
            await sceneManager.LoadSceneSynced(gameplayScene.AssetGUID);

            // Load map scene
            await sceneManager.LoadSceneSynced(lobbySettings.selectedMap.MapReference.AssetGUID);

            // Call setup event for gameplay

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
    }
}