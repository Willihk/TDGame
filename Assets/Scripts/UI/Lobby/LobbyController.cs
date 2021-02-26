using TDGame.Network;
using UnityEngine;

namespace TDGame.UI.Lobby
{
    public class LobbyController : MonoBehaviour
    {
        [SerializeField]
        private GameObject mainPanel;

        [SerializeField]
        private LobbyPlayerList lobbyPlayerList;
        
        public void OnClickStart()
        {
            if (TDGameNetworkManager.Instance.allPlayersReady)
            {
                TDGameNetworkManager.Instance.GotoGameScene();
            }
        }

        public void OnClickReady()
        {
            
        }
    }
}