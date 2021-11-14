using System;
using System.Linq;
using Mirror;
using TDGame.Events.Base;
using TDGame.Network.Messages.Player;
using UnityEngine;

namespace TDGame.Network.Player
{
    [Obsolete]
    public class PlayerManager : NetworkBehaviour
    {
        public static PlayerManager Instance;

        public readonly SyncList<RegisterPlayerData> PlayerDatas = new SyncList<RegisterPlayerData>();
        
        [SerializeField] private GameEvent clientPlayersChangedEvent;

        public void Awake()
        {
            Instance = this;
        }

        public override void OnStopClient()
        {
            Instance = null;
            base.OnStopClient();
        }

        public override void OnStopServer()
        {
            Instance = null;
            base.OnStopServer();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            UpdatePlayers();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            PlayerDatas.Callback += (op, index, item, newItem) =>
            {
                if (op == SyncList<RegisterPlayerData>.Operation.OP_CLEAR)
                    return;
                
                Debug.Log("Player data changed");
                clientPlayersChangedEvent.Raise();
            };
            clientPlayersChangedEvent.Raise();
        }

        [Server]
        public void UpdatePlayers()
        {
            PlayerDatas.Clear();
            PlayerDatas.AddRange(TDGameNetworkManager.Instance.connectedPlayers.Values.ToArray());
        }
    }
}