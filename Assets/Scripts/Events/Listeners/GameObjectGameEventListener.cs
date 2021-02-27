using TDGame.Events.Base;
using UnityEngine.Events;
using UnityEngine;

namespace TDGame.Events.Listeners
{
    public class GameObjectGameEventListener : BaseGameEventListener<GameObject, GameEvent<GameObject>, UnityEvent<GameObject>>
    {
    }
}
