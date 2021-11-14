﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace TDGame.Systems.TargetAcquisition
{
    [Obsolete]
    public abstract class BaseTargetAcquisitionSystem : MonoBehaviour
    {
        public abstract IEnumerable<GameObject> GetAvailableTargets();

        public abstract bool IsValidTarget(GameObject target);
    }
}