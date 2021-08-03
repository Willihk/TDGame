using System;
using System.Collections.Generic;
using UnityEngine;

namespace TDGame.Network.Player
{
    [CreateAssetMenu(fileName = "PlayerList", menuName = "Data/PlayerList", order = 0)]
    public class PlayerList : ScriptableObject
    {
        public List<int> Players;

        private void Awake()
        {
            Players = new List<int>();
        }

        public void AddPlayer(int id)
        {
            if (!Players.Contains(id))
                Players.Add(id);
        }

        public void RemovePlayer(int id)
        {
            Players.Remove(id);
        }
    }
}