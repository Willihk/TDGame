using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

namespace TDGame.Systems.Stats
{
    public class NetworkedStatsController : NetworkBehaviour
    {
        [SerializeField]
        private Transform statHolder;

        [Tooltip("Auto loads all stat wrappers from the stat holder")]
        public List<StatWrapper> stats;

        private void Awake()
        {
            statHolder ??= transform;

            stats ??= new List<StatWrapper>();
            stats.AddRange(statHolder.GetComponents<StatWrapper>());
        }

        public StatWrapper GetStatByName(string name)
        {
            if (stats.Any(x => x.stat.Name == name))
                return stats.First(x => x.name == name);

            return null;
        }
    }
}