using System.Collections;
using System.Collections.Generic;
using TDGame.Network;
using TDGame.Network.Player;
using UnityEngine;

namespace TDGame.UI.PlayerList
{
    public class PlayerList : MonoBehaviour
    {
        [SerializeField]
        GameObject entryPrefab;
        [SerializeField]
        Transform content;

        List<GameObject> cachedPlayerEntries = new List<GameObject>();

        public void UpdatePlayers()
        {
            if (!PlayerManager.Instance)
                return;

            Debug.Log("updating player list " + PlayerManager.Instance.PlayerDatas.Count);

            cachedPlayerEntries.ForEach(x => Destroy(x));
            cachedPlayerEntries.Clear();

            foreach (PlayerData player in PlayerManager.Instance.PlayerDatas)
            {
                AddPlayerEntry(player.Name);
            }
        }

        void AddPlayerEntry(string playerName)
        {
            print(playerName);
            var entryObject = Instantiate(entryPrefab, content);
            entryObject.GetComponent<PlayerListEntry>().Initialize(playerName);

            cachedPlayerEntries.Add(entryObject);
        }
    }
}