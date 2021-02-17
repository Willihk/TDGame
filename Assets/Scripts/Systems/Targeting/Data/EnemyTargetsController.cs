using System;
using System.Collections.Generic;
using UnityEngine;

namespace TDGame.Systems.Targeting.Data
{
    public class EnemyTargetsController : MonoBehaviour
    {
        public static EnemyTargetsController Instance;

        public List<GameObject> targets;
        private void Awake()
        {
            Instance ??= this;
        }

        public List<GameObject> GetTargets()
        {
            return targets;
        }
    }
}