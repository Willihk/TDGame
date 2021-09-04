using Mirror;
using TDGame.Events.Base;
using TDGame.Network.Components;
using UnityEngine;

namespace TDGame.UI.Lobby
{
    public class LobbyController : MonoBehaviour
    {
        private CustomNetworkManager manager;

        [SerializeField]
        private GameEvent startGameEvent;

        private void Start()
        {
            manager = CustomNetworkManager.Instance;
        }

        public void OnClickStart()
        {
            // Check host/server
            if (NetworkServer.active)
            {
                startGameEvent.Raise();
            }
        }
        
        public void OnClickReady()
        {
            startGameEvent.Raise();
        }
        
        public void OnClickLeave()
        {
            manager.Stop();
        }
    }
}