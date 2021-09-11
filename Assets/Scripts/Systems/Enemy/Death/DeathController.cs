using Mirror;
using TDGame.Systems.Economy.Old;
using TDGame.Systems.Enemy.Manager;
using TDGame.Systems.Stats;
using UnityEngine;

namespace TDGame.Systems.Enemy.Death
{
    public class DeathController : MonoBehaviour
    {
        [SerializeField]
        private NetworkedStatsController statsController;
        private StatWrapper currencyRewardStat;

        [SerializeField]
        private string statName = "CurrencyReward";

        private void Start()
        {
            currencyRewardStat = statsController.GetStatByName(statName);
        }
        
        public void OnDeath()
        {
            EnemyManager.Instance.UnregisterTarget(gameObject);
            NetworkServer.Destroy(gameObject);
            PlayerEconomyManager.Instance.AddCurrencyToAllPlayers((int) currencyRewardStat.stat.Value);
        }
    }
}