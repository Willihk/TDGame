using Mirror;
using System.Collections;
using TDGame.Events;
using UnityEngine;
using UnityEngine.Events;

namespace TDGame.Events.Network
{
    public class NetworkEventListener : BaseGameEventListener<NetworkConnection, NetworkGameEvent, UnityEvent<NetworkConnection>>
    {
    }
}