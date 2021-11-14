using System;
using TDGame.Systems.Targeting.Implementations;
using UnityEngine;

namespace TDGame.Systems.Targeting.Utility
{
    [Obsolete]
    public class LookAtTarget : MonoBehaviour
    {
        [SerializeField]
        private SingleTargetSystem targetSystem;

        [SerializeField]
        protected float turnRate = 20;

        [SerializeField]
        protected Transform partToRotate;

        private void Update()
        {
            RotateTowardsTarget();
        }

        protected void RotateTowardsTarget()
        {
            Vector3 dir = targetSystem.clientTargetPosition - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnRate)
                .eulerAngles;
            partToRotate.rotation = Quaternion.Euler(transform.eulerAngles.x, rotation.y, transform.eulerAngles.z);
        }
    }
}