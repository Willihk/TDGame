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

        public void ConnectToServer()
        {
            string ip = address.text;
            ip = ip.Replace("​", string.Empty);
            
            manager.StartClient(ip);
        }

        public void StartHost()
        {
            manager.StartHost();
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}
