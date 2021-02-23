using System.Collections.Generic;
using System.Linq;
using TDGame.Systems.Targeting.Base;
using Unity.Mathematics;
using UnityEngine;

namespace TDGame.Systems.Targeting.Implementations
{
    public class MultiTargetSystem : BaseTargetingSystem
    {
        [SerializeField]
        protected int maxTargets = 4;
        
        public IEnumerable<GameObject> GetTargets()
        {
            var localPosition = transform.position;

            var availableTargets = acquisitionSystem.GetAvailableTargets().ToArray();

            return availableTargets.OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).Take(maxTargets);
        }
        
        public bool IsValidTarget(GameObject target)
        {
            return acquisitionSystem.IsValidTarget(target);
        }
    }
}