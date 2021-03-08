using System;
using Mirror;
using TDGame.Systems.Health.Global;
using TDGame.Systems.Stats;
using TDGame.Systems.Targeting.Data;
using UnityEngine;

namespace TDGame.Systems.Enemy.ReachedEnd
{
    public class ReachedEndController : MonoBehaviour
    {
        private NetworkedStatsController statsController;
        private StatWrapper reachedEndDamageStat;
        
        [SerializeField]
        private string statName = "ReachedEndDamage";

        private void Start()
        {
            reachedEndDamageStat = statsController.GetStatByName(statName);
        }

        public void ReachedEnd()
        {
            GlobalHealthSystem.Instance.ReduceHealth(reachedEndDamageStat.stat.Value);
            EnemyTargetsController.Instance.targets.Remove(gameObject);
            NetworkServer.Destroy(gameObject);
        }
    }
}