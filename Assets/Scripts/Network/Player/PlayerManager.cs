using Mirror;
using System.Collections.Generic;
using System.Linq;
using TDGame.Events;
using TDGame.Network.Player;
using UnityEngine;
using UnityEngine.Events;

namespace TDGame.Network
{
    public class PlayerManager : NetworkBehaviour
    {
        public static PlayerManager Instance;

        public SyncList<PlayerData> PlayerDatas = new SyncList<PlayerData>();

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (Instance is null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        [Server]
        public void UpdatePlayers()
        {
            PlayerDatas.Clear();
            PlayerDatas.AddRange(TDGameNetworkManager.Instance.connectedPlayers.Values.ToArray());
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            UpdatePlayers();
        }
    }
}