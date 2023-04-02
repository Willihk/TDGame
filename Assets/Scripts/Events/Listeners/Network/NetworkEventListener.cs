using TDGame.Events.Base;
using TDGame.Events.Types.Network;
using TDGame.Network.Components.Messaging;
using UnityEngine.Events;

namespace TDGame.Events.Listeners.Network
{
    public class NetworkEventListener : BaseGameEventListener<TDNetworkConnection, NetworkGameEvent, UnityEvent<TDNetworkConnection>>
    {
    }
}