using TDGame.Events.Base;
using UnityEngine.Events;

namespace TDGame.Events.Listeners
{
    public class StringGameEventListener : BaseGameEventListener<string, GameEvent<string>, UnityEvent<string>>
    {
    }
}