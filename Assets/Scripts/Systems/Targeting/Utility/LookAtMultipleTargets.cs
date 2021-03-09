using System.Collections;
using System.Collections.Generic;
using TDGame.Systems.Targeting.Implementations;
using UnityEngine;

namespace TDGame.Systems.Targeting.Utility
{
    public class LookAtMultipleTargets : MonoBehaviour
    {
        [SerializeField]
        private MultiTargetSystem targetSystem;

        [SerializeField]
        protected float turnRate = 20;

        [SerializeField]
        protected List<Transform> partsToRotate;

        private void Update()
        {
            RotateTowardsTargets();
        }

        protected void RotateTowardsTargets()
        {
            for (int i = 0; 0 < targetSystem.syncedTargetPositions.Count; i++)
            {
                if (targetSystem.syncedTargetPositions.Count >= i)
                    return;
                Vector3 dir = targetSystem.syncedTargetPositions[i] - transform.position;
                Quaternion lookRotation = Quaternion.LookRotation(dir);
                Vector3 rotation = Quaternion.Lerp(partsToRotate[i].rotation, lookRotation, Time.deltaTime * turnRate)
                    .eulerAngles;
                partsToRotate[i].rotation = Quaternion.Euler(transform.eulerAngles.x, rotation.y, transform.eulerAngles.z);
            }
        }
    }
}
