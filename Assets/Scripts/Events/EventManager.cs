using System;
using TDGame.Events.Types.Network;
using UnityEngine;

namespace TDGame.Events
{
    public class EventManager : MonoBehaviour
    {
        public static EventManager Instance { get; private set; }


        public NetworkGameEvent onServerNetworkConnect;
        public NetworkGameEvent onServerNetworkDisconnect;

        private void Awake()
        {
            Instance = this;
        }
    }
}