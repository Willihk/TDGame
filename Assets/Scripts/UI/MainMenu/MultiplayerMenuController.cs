using Mirror;
using TMPro;
using UnityEngine;

namespace TDGame.UI.MainMenu
{
    public class MultiplayerMenuController : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI nameText;
        
        public void StartServer()
        {
            NetworkManager.singleton.StartServer();
        }
    }
}
