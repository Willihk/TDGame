using System.Collections.Generic;
using TDGame.Systems.Targeting.Data;
using UnityEngine;

namespace TDGame.Systems.TargetAcquisition
{
    public abstract class BaseTargetAcquisitionSystem : MonoBehaviour
    {
        public abstract IEnumerable<GameObject> GetAvailableTargets();

        public abstract bool IsValidTarget(GameObject target);
    }
}