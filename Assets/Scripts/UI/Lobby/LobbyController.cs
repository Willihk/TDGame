using System;
using TDGame.Network;
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
            if (manager)
            {
                
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