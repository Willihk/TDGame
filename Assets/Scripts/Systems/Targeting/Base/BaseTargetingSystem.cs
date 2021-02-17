using TDGame.Systems.TargetAcquisition;
using UnityEngine;

namespace TDGame.Systems.Targeting.Base
{
    public class BaseTargetingSystem : MonoBehaviour
    {
        [SerializeField]
        protected BaseTargetAcquisitionSystem acquisitionSystem;
    }
}