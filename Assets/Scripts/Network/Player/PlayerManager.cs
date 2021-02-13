using Mirror;
using System.Collections.Generic;
using System.Linq;
using TDGame.Network.Player;
using UnityEngine;
using UnityEngine.Events;

namespace TDGame.Network
{
    public class PlayerManager : NetworkBehaviour
    {
        public static PlayerManager Instance;

        public SyncList<PlayerData> PlayerDatas = new SyncList<PlayerData>();

        Dictionary<int, PlayerData> connectedPlayers = new Dictionary<int, PlayerData>();


        public UnityEvent OnPlayerListChange;

        private void Awake()
        {
            if (Instance is null)
                Instance = this;
            else
                Destroy(gameObject);

            OnPlayerListChange = new UnityEvent();
            DontDestroyOnLoad(Instance);
        }

        public void PlayerDisconnected(NetworkConnection conn)
        {
            var player = connectedPlayers[conn.connectionId];

            PlayerDatas.RemoveAll(x => x.Name == player.Name);
            OnPlayerListChange.Invoke();
        }

        public void PlayerConnected(PlayerData playerData)
        {
            Debug.Log(PlayerDatas);
            PlayerDatas.Add(playerData);
            OnPlayerListChange.Invoke();
        }
    }
}