using System.Collections.Generic;
using System.Linq;
using Mirror;
using TDGame.Network;
using TDGame.Network.Lobby;
using TDGame.Network.Player;
using TDGame.UI.PlayerList;
using UnityEngine;

namespace TDGame.UI.Lobby
{
    public class Old_LobbyPlayerList : NetworkBehaviour
    {
        [SerializeField]
        private GameObject entryPrefab;
        
        [SerializeField]
        Transform content;

        public SyncList<LobbyPlayerData> players = new SyncList<LobbyPlayerData>();

        List<GameObject> cachedPlayerEntries = new List<GameObject>();

        public override void OnStartClient()
        {
            players.Callback += (op, index, item, newItem) =>
            {
                if (op == SyncList<LobbyPlayerData>.Operation.OP_CLEAR)
                    return;
                
                Debug.Log("update lobby players");
                UpdatePlayers();
            };
            UpdatePlayers();
        }

        public override void OnStartServer()
        {
            GetPlayers();
        }

        public void UpdatePlayers()
        {
            cachedPlayerEntries.ForEach(x => Destroy(x));
            cachedPlayerEntries.Clear();

            foreach (var networkRoomPlayer in TDGameNetworkManager.Instance.roomSlots)
            {
                var player = (NetworkedLobbyPlayer) networkRoomPlayer;
                AddPlayerEntry(player);
            }
        }

        void AddPlayerEntry(NetworkedLobbyPlayer player)
        {
            var entryObject = Instantiate(entryPrefab, content);
            entryObject.GetComponent<LobbyPlayerListEntry>().Initialize(player.playerData.Name, player.readyToBegin);

            cachedPlayerEntries.Add(entryObject);
        }

        [Server]
        public void GetPlayers()
        {
            players.Clear();
            foreach (var player in TDGameNetworkManager.Instance.connectedPlayers.Values)
            {
                players.Add(new LobbyPlayerData {Name = player.Name});
            }
        }
    }

    public struct LobbyPlayerData
    {
        public string Name;
    }
}