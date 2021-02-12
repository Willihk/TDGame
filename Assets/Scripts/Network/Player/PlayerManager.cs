using Mirror;
using System.Collections.Generic;
using System.Linq;
using TDGame.Network.Player;
using UnityEngine;

namespace TDGame.Network
{
    public class PlayerManager : NetworkBehaviour
    {
        public static PlayerManager Instance;

        public SyncList<PlayerData> PlayerDatas = new SyncList<PlayerData>();


        private void Awake()
        {
            if (Instance is null)
                Instance = this;
            else
                Destroy(gameObject);

            DontDestroyOnLoad(Instance);
        }

        public void PlayerConnected(PlayerData playerData)
        {
            PlayerDatas.Add(playerData);
        }
    }
}