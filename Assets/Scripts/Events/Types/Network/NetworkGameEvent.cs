using System;
using TDGame.Events.Base;
using TDGame.Network.Components.Messaging;
using UnityEngine;

namespace TDGame.Events.Types.Network
{
    [CreateAssetMenu(fileName = "NetworkEvent", menuName = "GameEvents/NetworkEvent")]
    [Serializable]
    public class NetworkGameEvent : GameEvent<TDNetworkConnection>
    {
    }
}