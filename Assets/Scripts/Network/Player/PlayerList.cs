using System;
using System.Collections.Generic;
using TDGame.Events.Types;
using UnityEngine;
using UnityEngine.Serialization;

namespace TDGame.Network.Player
{
    [CreateAssetMenu(fileName = "PlayerList", menuName = "Data/Player/PlayerList", order = 0)]
    public class PlayerList : ScriptableObject
    {
        public List<int> players;

        public void Clear()
        {
            players = new List<int>();
        }

        public void AddPlayer(int id)
        {
            if (!players.Contains(id))
                players.Add(id);
        }

        public void RemovePlayer(int id)
        {
            players.Remove(id);
        }
    }
}