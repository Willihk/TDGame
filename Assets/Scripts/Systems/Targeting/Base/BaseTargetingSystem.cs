using Mirror;
using Sirenix.OdinInspector;
using TDGame.Systems.TargetAcquisition;
using UnityEngine;

namespace TDGame.Systems.Targeting.Base
{
    public class BaseTargetingSystem : NetworkBehaviour
    {
        [SerializeField]
        protected BaseTargetAcquisitionSystem acquisitionSystem;
    }
}