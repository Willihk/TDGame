using System;
using System.Collections.Generic;
using UnityEngine;

namespace TDGame.Systems.Target.Data
{
    [CreateAssetMenu(fileName = "TargetList", menuName = "Data/Target/TargetList", order = 0)]
    public class TargetList : ScriptableObject
    {
        [SerializeField]
        private List<GameObject> targets;

        private void Awake()
        {
            targets ??= new List<GameObject>();
        }

        public IEnumerable<GameObject> GetTargets()
        {
            return targets;
        }
    }
}