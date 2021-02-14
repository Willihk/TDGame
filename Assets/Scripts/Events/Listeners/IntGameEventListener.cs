using TDGame.Events.Base;
using UnityEngine.Events;

namespace TDGame.Events.Listeners
{
    public class IntGameEventListener : BaseGameEventListener<int,GameEvent<int>, UnityEvent<int>>
    {
    }
}