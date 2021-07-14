using Mirage;
using TDGame.Events.Base;
using TDGame.Events.Types.Network;
using UnityEngine.Events;

namespace TDGame.Events.Listeners.Network
{
    public class INetworkEventPlayerListener : BaseGameEventListener<INetworkPlayer, INetworkPlayerGameEvent, UnityEvent<INetworkPlayer>>
    {
    }
}