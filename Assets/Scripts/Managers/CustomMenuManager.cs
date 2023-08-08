using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TDGame.Network.Components;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace TDGame.Managers
{
    public class CustomMenuManager : MonoBehaviour
    {
        [SerializeField]
        public StandardSceneManager customSceneManager;

        [SerializeField]
        private string mainMenuScene;

        [SerializeField]
        private string roomScene;

        private void Start()
        {
            customSceneManager.LoadScene(mainMenuScene).Forget();
        }
        
        public void LoadRoomScene()
        {
            UniTask.Create(async () =>
            {
                await customSceneManager.UnLoadAllLoadedScenes();
                await customSceneManager.LoadScene(roomScene);
            });
        }

        public void ReturnToMainMenu()
        {
            UniTask.Create(async () =>
            {
                await customSceneManager.UnLoadAllLoadedScenes();
                await customSceneManager.LoadScene(mainMenuScene);
            });
        }
    }
}