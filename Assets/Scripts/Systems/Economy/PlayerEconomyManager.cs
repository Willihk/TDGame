using System;
using System.Collections.Generic;
using TDGame.Network.Player;
using UnityEngine;

namespace TDGame.Systems.Economy
{
    public class PlayerEconomyManager : MonoBehaviour
    {
        [SerializeField]
        private PlayerList playerList;

        private List<NetworkedEconomy> economies = new List<NetworkedEconomy>();

        private void Start()
        {
            foreach (var player in playerList.players)
            {
                CreateEconomy(player);
            }
        }

        void CreateEconomy(int playerId)
        {
            var gameObject = new GameObject("Economy - " + playerId);
            gameObject.transform.SetParent(transform);

            var economy = gameObject.AddComponent<NetworkedEconomy>();
            economies.Add(economy);
        }
    }
}