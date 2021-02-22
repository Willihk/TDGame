using Mirror;
using TMPro;
using UnityEngine;

namespace TDGame.UI.MainMenu
{
    public class MultiplayerMenuController : MonoBehaviour
    {
        NetworkManager manager;

        private void Start()
        {
            manager = NetworkManager.singleton;
        }

        [SerializeField]
        private TextMeshProUGUI nameText;
        [SerializeField]
        private TextMeshProUGUI address;

        public void StartServer()
        {
            manager.StartServer();
        }

        public void StartClient()
        {
            manager.networkAddress = address.text;
            manager.StartClient();
        }

        public void StartHost()
        {
            manager.StartHost();
        }
    }
}
