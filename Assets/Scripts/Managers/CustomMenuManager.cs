using TDGame.Network.Components;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

namespace TDGame.Managers
{
    public class CustomMenuManager : MonoBehaviour
    {
        [SerializeField]
        private CustomSceneManager customSceneManager;
        
        [SerializeField]
        private AssetReference mainMenuScene;
        [SerializeField]
        private AssetReference roomScene;
        
        public void LoadRoomScene()
        {
            customSceneManager.SwitchScenes(mainMenuScene.AssetGUID, roomScene.AssetGUID);
        }
    }
}