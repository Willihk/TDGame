using TDGame.Network.Components;
using TMPro;
using UnityEngine;

namespace TDGame.UI.MainMenu
{
    public class MainMenuController : MonoBehaviour
    {
        CustomNetworkManager manager;

        private void Start()
        {
            manager = CustomNetworkManager.Instance;
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
            string ip = address.text;
            ip = ip.Replace("​", string.Empty);
            
            manager.ConnectToServer(ip);
        }

        public void StartHost()
        {
            manager.StartHost();
        }
    }
}
