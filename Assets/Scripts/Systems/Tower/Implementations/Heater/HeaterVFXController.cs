using System;
using TDGame.Systems.Stats;
using UnityEngine;
using UnityEngine.VFX;

namespace TDGame.Systems.Tower.Implementations.Heater
{
    public class HeaterVFXController : MonoBehaviour
    {
        [SerializeField]
        private VisualEffect flowEffect;
        
        [SerializeField]
        protected NetworkedStatsController statsController;
        
        [SerializeField]
        protected string rangeStatName = "Range";
        
        protected StatWrapper rangeStat;

        private void Start()
        {
            foreach (var statsControllerStat in statsController.stats)
            {
                Debug.Log(statsControllerStat.stat.Name);
            }
            rangeStat = statsController.GetStatByName(rangeStatName);
            Setup();
        }
        

        public void Setup()
        {
            flowEffect.SetFloat("Range", rangeStat.stat.Value);
        }
    }
}