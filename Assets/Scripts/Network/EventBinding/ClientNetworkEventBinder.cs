using Mirror;
using TDGame.Events;
using TDGame.Network.Player;
using UnityEngine;

namespace TDGame.Network.EventBinding
{
    public class ClientNetworkEventBinder : NetworkBehaviour
    {
        [Header("Client Event Bindings")] 
        [SerializeField]
        GameEvent<PlayerData> onPlayerConnect;

        [SerializeField] 
        GameEvent<PlayerData> onPlayerDisconnect;

        #region Events
        
        [ClientRpc]
        public void PlayerConnected(PlayerData data)
        {
            onPlayerConnect.Raise(data);
        }

        [ClientRpc]
        public void PlayerDisconnected(PlayerData data)
        {
            onPlayerDisconnect.Raise(data);
        }

        #endregion
    }
}