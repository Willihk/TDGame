using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TDGame.Systems.Enemy.Manager;
using TDGame.Systems.Stats;
using UnityEngine;

namespace TDGame.Systems.TargetAcquisition.Implementations
{
    public class RangeTargetAcquisition : BaseTargetAcquisitionSystem
    {
        [SerializeField]
        protected string rangeStatName = "Range";
        
        [ReadOnly]
        public StatWrapper rangeStat;

        public override IEnumerable<GameObject> GetAvailableTargets()
        {
            var localPosition = transform.position;
            return EnemyManager.Instance.GetTargets().Where(x => Vector3.Distance(x.transform.position, localPosition) <= rangeStat.stat.Value);
        }

        public override bool IsValidTarget(GameObject target)
        {
            return Vector3.Distance(target.transform.position, transform.position) <= rangeStat.stat.Value;
        }
    }
}