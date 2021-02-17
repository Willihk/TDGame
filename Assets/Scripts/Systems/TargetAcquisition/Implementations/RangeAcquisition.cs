using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TDGame.Systems.TargetAcquisition.Implementations
{
    public class RangeAcquisition : BaseTargetAcquisition
    {
        [SerializeField]
        protected float range;

        public override IEnumerable<GameObject> GetAvailableTargets()
        {
            var localPosition = transform.position;
            return targetList.GetTargets().Where(x => Vector3.Distance(x.transform.position, localPosition) <= range);
        }
    }
}