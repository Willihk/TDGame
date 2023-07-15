using System;
using TDGame.Events.Base;
using TDGame.Events.Types;
using TDGame.Events.Types.Network;
using UnityEngine;
using UnityEngine.Serialization;

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
        public GameEvent onClickStartGame;
        
        /// <summary>
        /// Value is tower id
        /// </summary>
        public IntGameEvent onClickTower;
        
        /// <summary>
        /// tower id, hash of upgrade 
        /// </summary>
        public IntHashGameEvent onClickTowerUpgrade;

        private void Awake()
        {
            Instance = this;
        }
    }
}