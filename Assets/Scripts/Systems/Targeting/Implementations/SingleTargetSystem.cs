using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Sirenix.OdinInspector;
using TDGame.Systems.Targeting.Base;
using Unity.Mathematics;
using UnityEngine;

namespace TDGame.Systems.Targeting.Implementations
{
    public class SingleTargetSystem : BaseTargetingSystem
    {
        [ReadOnly]
        public GameObject target;

        [SyncVar]
        [ReadOnly]
        public Vector3 clientTargetPosition;

        public GameObject GetTarget()
        {
            var localPosition = transform.position;

            var availableTargets = acquisitionSystem.GetAvailableTargets().ToArray();

            int closestIndex = 0;
            float closestDistance = math.INFINITY;
            for (int i = 0; i < availableTargets.Length; i++)
            {
                float distance = Vector3.Distance(availableTargets[i].transform.position, localPosition);
                if (distance > closestDistance)
                    continue;

                closestDistance = distance;
                closestIndex = i;
            }

            return availableTargets.Length > 0 ? availableTargets[closestIndex] : null;
        }

        private void Update()
        {
            if (!isServer)
                return;

            if (target != null)
            {
                if (!IsValidTarget(target))
                {
                    target = null;
                    return;
                }

                clientTargetPosition = target.transform.Find("TargetPoint").position;
            }
            else
            {
                target = GetTarget();
            }
        }

        public bool IsValidTarget(GameObject target)
        {
            return acquisitionSystem.IsValidTarget(target);
        }
    }
}