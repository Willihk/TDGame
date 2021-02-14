using System;
using Mirror;
using TDGame.Events.Base;
using UnityEngine;

namespace TDGame.Events.Types.Network
{
    [CreateAssetMenu(fileName = "NetworkEvent", menuName = "GameEvents/NetworkEvent")]
    [Serializable]
    public class NetworkGameEvent : GameEvent<NetworkConnection>
    {
    }
}