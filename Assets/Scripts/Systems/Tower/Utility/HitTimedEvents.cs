using System;
using TDGame.Systems.Stats;
using UnityEngine;
using UnityEngine.Events;

namespace TDGame.Systems.Tower.Utility
{
    public class HitTimedEvents : MonoBehaviour
    {
        [SerializeField]
        public UnityEvent TriggerHitEvent;
        
        [SerializeField]
        private NetworkedStatsController statsController;

        [SerializeField]
        private string hitRateStatName = "HitRate";

        [SerializeField]
        private bool recurringTimer = true;
        
        private StatWrapper hitRateStat;

        private void Awake()
        {
            TriggerHitEvent ??= new UnityEvent();
        }

        private void Start()
        {
            hitRateStat = statsController.GetStatByName(hitRateStatName);
            StartNewEventTimer();
        }

        public void StartNewEventTimer()
        {
            Invoke(nameof(TriggerEvent), hitRateStat.stat.Value);
        }

        public void TriggerEvent()
        {
            TriggerHitEvent.Invoke();
            if (recurringTimer)
            {
                StartNewEventTimer();
            }
        }
    }
}