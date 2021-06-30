using System;
using Mirror;
using TDGame.Network;
using TDGame.Network.Lobby;
using UnityEngine;

namespace TDGame.UI.Lobby
{
    public class LobbyController : MonoBehaviour
    {
        private TDGameNetworkManager manager;

        [SerializeField]
        private GameObject mainPanel;

        [SerializeField]
        private LobbyPlayerList lobbyPlayerList;

        private void Start()
        {
            manager = TDGameNetworkManager.Instance;
        }

        public void OnClickStart()
        {
            if (manager.allPlayersReady)
            {
                manager.GotoGameScene();
            }
        }

        public void OnClickReady()
        {
            if (NetworkedLobbyPlayer.LocalPlayer)
            {
                NetworkedLobbyPlayer.LocalPlayer.CmdChangeReadyState(!NetworkedLobbyPlayer.LocalPlayer.readyToBegin);
            }
        }

        public void OnClickLeave()
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                // stop host if host-only
                manager.StopHost();
            }
            else if (NetworkClient.isConnected)
            {
                // stop client if client-only
                manager.StopClient();
            }
            else if (NetworkServer.active)
            {
                // stop server if server-only
                manager.StopServer();
            }
        }
    }
}