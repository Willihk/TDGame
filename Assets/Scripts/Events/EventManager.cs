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
        
        /// <summary>
        /// Value is the new health
        /// </summary>
        public IntGameEvent onPlayerHealthChanged;


        /// <summary>
        /// Value is the wave number
        /// </summary>
        public IntGameEvent onWaveCompleted;
        /// <summary>
        /// Value is the wave number
        /// </summary>
        public IntGameEvent onWaveStarted;

        public GameEvent onClickNextWave;

        private void Awake()
        {
            Instance = this;
        }
    }
}