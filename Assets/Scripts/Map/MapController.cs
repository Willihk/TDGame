using TDGame.Network.Components;
using TDGame.Network.Components.Interfaces;
using TDGame.Settings;
using UnityEngine;

namespace TDGame.Map
{
    public class MapController : MonoBehaviour
    {
        [SerializeField]
        private LobbySettings settings;

        private ICustomSceneManager sceneManager;
        private void Start()
        {
            sceneManager = CustomSceneManager.Instance;
            var map = settings.selectedMap;
            
            sceneManager.LoadSceneSynced(map.MapReference.AssetGUID);
        }
    }
}
