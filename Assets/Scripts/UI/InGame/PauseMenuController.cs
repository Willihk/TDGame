using System;
using Doozy.Engine;
using Mirror;
using TDGame.Cursor;
using TDGame.Network;
using UnityEngine;

namespace TDGame.UI.InGame
{
    public class PauseMenuController : MonoBehaviour
    {
        [SerializeField]
        private LocalCursorState cursorState;

        private TDGameNetworkManager manager;

        private void Start()
        {
            manager = TDGameNetworkManager.Instance;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                switch (cursorState.State)
                {
                    case CursorState.None:
                        cursorState.State = CursorState.Menu;
                        GameEventMessage.SendEvent("Open PauseMenu");
                        break;
                    case CursorState.Menu:
                        cursorState.State = CursorState.None;
                        GameEventMessage.SendEvent("Close PauseMenu");
                        break;
                }
            }
        }

        public void Disconnect()
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
