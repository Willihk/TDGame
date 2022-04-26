using TDGame.Network;
using TMPro;
using UnityEngine;

namespace TDGame.UI.MainMenu
{
    public class MultiplayerMenuController : MonoBehaviour
    {
        GameNetworkManager manager;

        private void Start()
        {
            manager = GameNetworkManager.Instance;
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
            manager.networkAddress = ip;
            manager.StartClient();
        }

        public void StartHost()
        {
            manager.StartHost();
        }
    }
}
