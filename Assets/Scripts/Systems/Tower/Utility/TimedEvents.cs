using System;
using TDGame.Systems.Stats;
using UnityEngine;
using UnityEngine.Events;

namespace TDGame.Systems.Tower.Utility
{
    public class TimedEvents : MonoBehaviour
    {
        [SerializeField]
        public UnityEvent timeReachedEvent;
        
        [SerializeField]
        private NetworkedStatsController statsController;

        [SerializeField]
        private string delayStatName = "HitRate";

        [SerializeField]
        private bool recurringTimer = true;
        
        private StatWrapper delayStat;

        private void Awake()
        {
            timeReachedEvent ??= new UnityEvent();
        }

        private void Start()
        {
            delayStat = statsController.GetStatByName(delayStatName);
            StartNewEventTimer();
        }

        public void StartNewEventTimer()
        {
            Invoke(nameof(TriggerEvent), delayStat.stat.Value);
        }

        public void TriggerEvent()
        {
            timeReachedEvent.Invoke();
            if (recurringTimer)
            {
                StartNewEventTimer();
            }
        }
    }
}