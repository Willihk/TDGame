using Mirror;
using TDGame.Events.Base;
using TDGame.Events.Types.Network;
using UnityEngine.Events;

namespace TDGame.Events.Listeners.Network
{
    public class NetworkEventListener : BaseGameEventListener<NetworkConnection, NetworkGameEvent, UnityEvent<NetworkConnection>>
    {
    }
}