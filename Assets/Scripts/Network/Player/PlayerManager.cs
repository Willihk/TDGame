using Mirror;
using System.Collections.Generic;
using System.Linq;
using TDGame.Events;
using TDGame.Network.EventBinding;
using TDGame.Network.Message.Player;
using TDGame.Network.Player;
using UnityEngine;
using UnityEngine.Events;

namespace TDGame.Network
{
    public class PlayerManager : NetworkBehaviour
    {
        public static PlayerManager Instance;

        public SyncList<PlayerData> PlayerDatas = new SyncList<PlayerData>();

        [SerializeField]
        private GameEvent PlayersChangedEvent;

        public void Awake()
        {
            if (Instance is null)
                Instance = this;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            PlayerDatas.Callback += (op, index, item, newItem) =>
            {
                Debug.Log("Player data changed ");
                PlayersChangedEvent.Raise();
            };
            PlayersChangedEvent.Raise();
        }

        [Server]
        public void UpdatePlayers()
        {
            PlayerDatas.Clear();
            PlayerDatas.AddRange(TDGameNetworkManager.Instance.connectedPlayers.Values.ToArray());
            //NetworkServer.SendToAll(new PlayersChangedMessage());
        }

        [Server]
        public void PlayerConnected(NetworkConnection connection)
        {
            var connected = TDGameNetworkManager.Instance.connectedPlayers[connection.connectionId];
            PlayerDatas.Add(TDGameNetworkManager.Instance.connectedPlayers[connection.connectionId]);
            
            //NetworkServer.SendToAll(new PlayersChangedMessage());
        }

        [Server]
        public void PlayerDisconnected()
        {
            PlayerDatas.Clear();
            PlayerDatas.AddRange(TDGameNetworkManager.Instance.connectedPlayers.Values.ToArray());

            //eventBinder.RpcPlayerDisconnected(new PlayerData{Name = "Unknown"});
            //NetworkServer.SendToAll(new PlayersChangedMessage());
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            UpdatePlayers();
        }
    }
}