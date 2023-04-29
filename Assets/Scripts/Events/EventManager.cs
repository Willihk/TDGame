using System;
using TDGame.Events.Base;
using TDGame.Events.Types;
using TDGame.Events.Types.Network;
using UnityEngine;

namespace TDGame.Events
{
    public class EventManager : MonoBehaviour
    {
        public static EventManager Instance { get; private set; }


        public NetworkGameEvent onServerNetworkConnect;
        public NetworkGameEvent onServerNetworkDisconnect;


        public Hash128GameEvent onBeginPlacement;
        
        
        /// <summary>
        /// Value is player id
        /// </summary>
        public IntGameEvent onEconomyChanged;

        public VoidGameEvent onMapLoaded;
        public VoidGameEvent onGridInitialized;
        public VoidGameEvent onPathRegistered;

        private void Awake()
        {
            Instance = this;
        }
    }
}