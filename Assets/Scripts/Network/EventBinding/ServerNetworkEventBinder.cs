using Mirror;
using TDGame.Events.Types.Network;
using UnityEngine;

namespace TDGame.Network.EventBinding
{
    public class ServerNetworkEventBinder : MonoBehaviour
    {
        [Header("Server Event Bindings")] [SerializeField]
        NetworkGameEvent serverOnClientConnectEvent;

        [SerializeField] 
        NetworkGameEvent serverOnClientDisconnectEvent;

        #region Server Events

        public void ServerOnClientDisconnect(NetworkConnection connection)
        {
            serverOnClientDisconnectEvent.Raise(connection);
        }

        public void ServerOnClientConnect(NetworkConnection connection)
        {
            serverOnClientConnectEvent.Raise(connection);
        }

        #endregion

    }
}