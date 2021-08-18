using System;
using TDGame.Events.Base;
using TDGame.Network;
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
            if (manager.mirageManager.Server.Active)
            {
                startGameEvent.Raise();
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