using System;
using System.Collections;
using TDGame.Network.Player;
using UnityEngine;

namespace TDGame.Events.Network
{
    [CreateAssetMenu(fileName = "GameEvent", menuName = "GameEvents/PlayerEvent")]
    [Serializable]
    public class PlayerGameEvent : GameEvent<PlayerData>
    {
    }
}