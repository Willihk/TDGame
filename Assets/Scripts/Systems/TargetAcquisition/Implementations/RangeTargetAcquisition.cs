using System.Collections.Generic;
using System.Linq;
using TDGame.Systems.Targeting.Data;
using UnityEngine;

namespace TDGame.Systems.TargetAcquisition.Implementations
{
    public class RangeTargetAcquisition : BaseTargetAcquisitionSystem
    {
        [SerializeField]
        protected float range;

        public override IEnumerable<GameObject> GetAvailableTargets()
        {
            var localPosition = transform.position;
            return EnemyTargetsController.Instance.GetTargets().Where(x => Vector3.Distance(x.transform.position, localPosition) <= range);
        }

        public override bool IsValidTarget(GameObject target)
        {
            return Vector3.Distance(target.transform.position, transform.position) <= range;
        }
    }
}