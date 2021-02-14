using Mirror;
using System.Collections.Generic;
using System.Linq;
using TDGame.Events;
using TDGame.Network.EventBinding;
using TDGame.Network.Player;
using UnityEngine;
using UnityEngine.Events;
using static TDGame.Network.EventBinding.ClientNetworkEventBinder;

namespace TDGame.Network
{
    public class PlayerManager : NetworkBehaviour
    {
        public static PlayerManager Instance;

        public SyncList<PlayerData> PlayerDatas = new SyncList<PlayerData>();

        [SerializeField]
        private ClientNetworkEventBinder eventBinder;

        public void Awake()
        {
            if (Instance is null)
                Instance = this;
        }

        [Server]
        public void UpdatePlayers()
        {
            PlayerDatas.Clear();
            PlayerDatas.AddRange(TDGameNetworkManager.Instance.connectedPlayers.Values.ToArray());
            NetworkServer.SendToAll(new PlayersChangedMessage());
        }

        [Server]
        public void PlayerConnected(NetworkConnection connection)
        {
            var connected = TDGameNetworkManager.Instance.connectedPlayers[connection.connectionId];
            PlayerDatas.Add(TDGameNetworkManager.Instance.connectedPlayers[connection.connectionId]);
            
            //eventBinder.RpcPlayerConnected(connected);
            NetworkServer.SendToAll(new PlayersChangedMessage());
        }

        [Server]
        public void PlayerDisconnected()
        {
            PlayerDatas.Clear();
            PlayerDatas.AddRange(TDGameNetworkManager.Instance.connectedPlayers.Values.ToArray());

            //eventBinder.RpcPlayerDisconnected(new PlayerData{Name = "Unknown"});
            NetworkServer.SendToAll(new PlayersChangedMessage());
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            UpdatePlayers();
        }
    }
}