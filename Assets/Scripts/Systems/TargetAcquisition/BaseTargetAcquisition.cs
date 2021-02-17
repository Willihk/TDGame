using System.Collections.Generic;
using TDGame.Systems.Target.Data;
using UnityEngine;

namespace TDGame.Systems.TargetAcquisition
{
    public abstract class BaseTargetAcquisition : MonoBehaviour
    {
        [SerializeField]
        protected TargetList targetList;
        
        public abstract IEnumerable<GameObject> GetAvailableTargets();
    }
}