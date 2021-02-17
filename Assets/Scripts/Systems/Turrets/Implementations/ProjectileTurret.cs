﻿using System;
using Mirror;
using TDGame.Systems.Projectiles;
using TDGame.Systems.Targeting.Implementations;
using TDGame.Systems.Turrets.Base;
using UnityEngine;

namespace TDGame.Systems.Turrets.Implementations
{
    public class ProjectileTurret : BaseNetworkedTurret
    {
        [SerializeField]
        protected SingleTargetSystem targetSystem;

        [SerializeField]
        protected GameObject projectilePrefab;

        [Header("Stats")]
        [Space(10)]
        [SerializeField]
        protected float hitDamage = 5;

        [SerializeField]
        protected float turnRate = 20;

        [SerializeField]
        protected float fireRate = .4f;

        [Header("Visual")]
        [Space(10)]
        [SerializeField]
        protected Transform firePoint;

        [SerializeField]
        protected Transform partToRotate;

        [SerializeField]
        [SyncVar]
        private GameObject target;

        private float nextFire;

        void ShootProjectile()
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            var bulletComponent = projectile.GetComponent<TurretProjectile>();
            bulletComponent.Setup(target, hitDamage);
            nextFire = Time.time + fireRate;

            Rpc_ShootDummyProjectile();
        }

        [ClientRpc]
        private void Rpc_ShootDummyProjectile()
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            var bulletComponent = projectile.GetComponent<TurretProjectile>();
            bulletComponent.Setup(target, 0);
        }

        private void Update()
        {
            if (target != null)
            {
                LookAtTarget();

                if (!targetSystem.IsValidTarget(target))
                {
                    target = null;
                    return;
                }

                if (isServer)
                {
                    if (nextFire < Time.time)
                    {
                        ShootProjectile();
                    }
                }
            }
            else
            {
                if (isServer)
                    target = targetSystem.GetTarget();
            }
        }

        protected void LookAtTarget()
        {
            Vector3 dir = target.transform.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnRate)
                .eulerAngles;
            partToRotate.rotation = Quaternion.Euler(transform.eulerAngles.x, rotation.y, transform.eulerAngles.z);
        }
    }
}