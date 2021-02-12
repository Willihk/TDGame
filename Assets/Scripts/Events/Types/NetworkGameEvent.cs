using Mirror;
using System;
using UnityEngine;

namespace TDGame.Events
{
    [CreateAssetMenu(fileName = "NetworkEvent", menuName = "GameEvents/NetworkEvent")]
    [Serializable]
    public class NetworkGameEvent : GameEvent<NetworkConnection>
    {
    }
}