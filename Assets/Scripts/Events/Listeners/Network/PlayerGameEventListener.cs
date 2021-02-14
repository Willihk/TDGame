using System.Collections;
using TDGame.Events;
using TDGame.Network.Player;
using UnityEngine;
using UnityEngine.Events;

namespace TDGame.Events
{
    public class PlayerGameEventListener : BaseGameEventListener<PlayerData, GameEvent<PlayerData>, UnityEvent<PlayerData>>
    {
    }
}