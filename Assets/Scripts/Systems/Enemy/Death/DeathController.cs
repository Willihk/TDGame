using Mirror;
using TDGame.Systems.Economy;
using TDGame.Systems.Stats;
using TDGame.Systems.Targeting.Data;
using UnityEngine;

namespace TDGame.Systems.Enemy.Death
{
    public class DeathController : MonoBehaviour
    {
        private NetworkedStatsController statsController;
        private StatWrapper currencyRewardStat;

        private void Start()
        {
            currencyRewardStat = statsController.GetStatByName("ReachedEndDamage");
        }
        
        public void OnDeath()
        {
            EnemyTargetsController.Instance.targets.Remove(gameObject);
            NetworkServer.Destroy(gameObject);
            PlayerEconomyManager.Instance.AddCurrencyToAllPlayers((int) currencyRewardStat.stat.Value);
        }
    }
}