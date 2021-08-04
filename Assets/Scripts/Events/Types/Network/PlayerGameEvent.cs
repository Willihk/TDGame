using System;
using TDGame.Events.Base;
using TDGame.Network.Messages.Player;
using TDGame.Network.Player;
using UnityEngine;

namespace TDGame.Events.Types.Network
{
    [CreateAssetMenu(fileName = "GameEvent", menuName = "GameEvents/PlayerEvent")]
    [Serializable]
    public class PlayerGameEvent : GameEvent<PlayerData>
    {
    }
}