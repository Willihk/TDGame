using TDGame.Network.Components;
using TDGame.Network.Components.Interfaces;
using TDGame.Settings;
using UnityEngine;
using UnityEngine.Events;

namespace TDGame.Map
{
    public class MapController : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent mapLoaded;

        [SerializeField]
        private LobbySettings settings;

        private ICustomSceneManager sceneManager;
        
        private async void Start()
        {
            sceneManager = CustomSceneManager.Instance;
            var map = settings.selectedMap;
            
            await sceneManager.LoadSceneSynced(map.MapReference.AssetGUID);
            mapLoaded?.Invoke();
        }
    }
}
