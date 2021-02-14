using TDGame.Events.Base;
using TDGame.Network.Player;
using UnityEngine.Events;

namespace TDGame.Events.Listeners.Network
{
    public class PlayerGameEventListener : BaseGameEventListener<PlayerData, GameEvent<PlayerData>, UnityEvent<PlayerData>>
    {
    }
}