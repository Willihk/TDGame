using System.Linq;
using Cysharp.Threading.Tasks;
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

        private void Start()
        {
            customSceneManager.LoadScene(mainMenuScene.AssetGUID).Forget();
        }
        
        public void LoadRoomScene()
        {
            UniTask.Create(async () =>
            {
                await customSceneManager.UnLoadAllLoadedScenes();
                await customSceneManager.LoadScene(roomScene.AssetGUID);
            });
        }

        public void ReturnToMainMenu()
        {
            UniTask.Create(async () =>
            {
                await customSceneManager.UnLoadAllLoadedScenes();
                await customSceneManager.LoadScene(mainMenuScene.AssetGUID);
            });
        }
    }
}