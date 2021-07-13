using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TDGame.Initialization
{
    public class GameInitialization : MonoBehaviour
    {
        [SerializeField]
        private AssetReference mainScene;

        private void Start()
        {
            var ting = mainScene.LoadSceneAsync();
        }
    }
}