using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TDGame.Initialization
{
    public class GameInitialization : MonoBehaviour
    {
        [SerializeField]
        private AssetReference mainScene;

        private async void Start()
        {
            await UniTask.Delay(1000);
            var ting = mainScene.LoadSceneAsync();
        }
    }
}