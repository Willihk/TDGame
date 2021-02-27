using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using TDGame.Systems.Targeting.Base;
using Unity.Mathematics;
using UnityEngine;

namespace TDGame.Systems.Targeting.Implementations
{
    public class MultiTargetSystem : BaseTargetingSystem
    {
        public int maxTargets = 4;
        
        public List<GameObject> targets = new List<GameObject>();
        
        public SyncList<Vector3> syncedTargetPositions = new SyncList<Vector3>();

        private void Update()
        {
            if (!isServer)
                return;
                
            if (IsTargetUpdateNeeded())
            {
                UpdateTargets();
            }

            syncedTargetPositions.Clear();
            syncedTargetPositions.AddRange(targets.Select(x => x.transform.position));
        }

        public IEnumerable<GameObject> GetTargets()
        {
            return GetTargets(maxTargets);
        }

        public IEnumerable<GameObject> GetTargets(int amount)
        {
            var localPosition = transform.position;

            var availableTargets = acquisitionSystem.GetAvailableTargets().ToArray();

            return availableTargets.OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).Take(amount);
        }
        
        public bool IsValidTarget(GameObject target)
        {
            return acquisitionSystem.IsValidTarget(target);
        }
        
        void UpdateTargets()
        {
            targets.Clear();
            targets.AddRange(GetTargets());
        }

        bool IsTargetUpdateNeeded()
        {
            return targets.Any(x => x == null) || targets.Count < maxTargets || targets.Any(x => !IsValidTarget(x));
        }
    }
}