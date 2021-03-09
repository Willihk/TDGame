using System;
using Mirror;
using TDGame.Systems.Enemy.Manager;
using TDGame.Systems.Health.Global;
using TDGame.Systems.Stats;
using UnityEngine;

namespace TDGame.Systems.Enemy.ReachedEnd
{
    public class ReachedEndController : MonoBehaviour
    {
        [SerializeField]
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
            EnemyManager.Instance.UnregisterTarget(gameObject);
            NetworkServer.Destroy(gameObject);
        }
    }
}