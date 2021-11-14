using System;
using Mirror;
using Sirenix.OdinInspector;
using TDGame.Systems.TargetAcquisition;
using UnityEngine;

namespace TDGame.Systems.Targeting.Base
{
    [Obsolete]
    public class BaseTargetingSystem : NetworkBehaviour
    {
        [SerializeField]
        protected BaseTargetAcquisitionSystem acquisitionSystem;
    }
}