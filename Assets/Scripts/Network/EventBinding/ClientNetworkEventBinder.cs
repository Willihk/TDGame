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
        
        [SerializeField] 
        GameEvent onPlayersChanged;

        public struct PlayersChangedMessage : NetworkMessage
        {

        }

        public void Awake()
        {
            NetworkClient.RegisterHandler<PlayersChangedMessage>(PlayersChanged);
        }

        #region Events

        [ClientRpc]
        public void RpcPlayerConnected(PlayerData data)
        {
            onPlayerConnect.Raise(data);
            print(data  + " connected");
        }

        [ClientRpc]
        public void RpcPlayerDisconnected(PlayerData data)
        {
            onPlayerDisconnect.Raise(data);
        }

        public void PlayersChanged(NetworkConnection conn, PlayersChangedMessage message)
        {
            print("Calling players changed");
            onPlayersChanged.Raise();
        }

        [ClientRpc]
        void RpcPlayersChanged()
        {
            print("players changed");
            onPlayersChanged.Raise();
        }

        #endregion
    }
}