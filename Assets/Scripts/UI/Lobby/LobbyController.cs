using TDGame.Events;
using TDGame.Events.Base;
using TDGame.Network.Components;
using UnityEngine;

namespace TDGame.UI.Lobby
{
    public class LobbyController : MonoBehaviour
    {
        private CustomNetworkManager manager;

        private void Start()
        {
            manager = CustomNetworkManager.Instance;
        }

        public void OnClickStart()
        {
            // Check host/server
            if (CustomNetworkManager.Instance.serverWrapper.isListening)
            {
                EventManager.Instance.onClickStartGame.Raise();
            }
        }
        
        public void OnClickReady()
        {
        }
        
        public void OnClickLeave()
        {
            manager.Stop();
        }
    }
}