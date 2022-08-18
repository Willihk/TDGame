using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TDGame.Systems.TargetAcquisition.Implementations
{
    [Obsolete]
    public class FixedTargetAcquisition : BaseTargetAcquisitionSystem
    {
        [ReadOnly]
        public GameObject Target;

        public void SetTarget(GameObject target)
        {
            Target = target;
        }
        
        public override IEnumerable<GameObject> GetAvailableTargets()
        {
            return Target ? new[] {Target} : new GameObject[0];
        }

        public override bool IsValidTarget(GameObject target)
        {
            return target;
        }
    }
}