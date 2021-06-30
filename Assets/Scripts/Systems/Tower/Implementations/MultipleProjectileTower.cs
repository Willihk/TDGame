using System.Collections.Generic;
using Mirror;
using TDGame.Systems.Projectiles;
using TDGame.Systems.Stats;
using TDGame.Systems.Targeting.Implementations;
using TDGame.Systems.Tower.Base;
using UnityEngine;

namespace TDGame.Systems.Tower.Implementations
{
    public class MultipleProjectileTower : BaseNetworkedTower
    {
        [SerializeField]
        protected MultiTargetSystem targetSystem;

        [SerializeField]
        protected GameObject projectilePrefab;

        [Header("Stats")]
        [Space(10)]
        [SerializeField]
        protected StatWrapper hitDamage;

        [SerializeField]
        protected StatWrapper fireRate;

        [SerializeField]
        protected StatWrapper projectileSpeed;

        [Header("Visual")]
        [Space(10)]
        [SerializeField]
        protected List<Transform> firePoints;

        private float nextFire;

        void Update()
        {
            if (targetSystem.targets != null && isServer)
            {
                if (nextFire < Time.time)
                {
                    ShootProjectiles();
                }
            }
        }

        private void ShootProjectiles()
        {
            for (int i = 0; i < targetSystem.syncedTargetPositions.Count; i++)
            {
                GameObject projectile = Instantiate(projectilePrefab, firePoints[i].position, Quaternion.identity);
                var bulletComponent = projectile.GetComponent<TurretProjectile>();
                bulletComponent.Setup(targetSystem.syncedTargetPositions[i], hitDamage.stat.Value, projectileSpeed.stat.Value);
                nextFire = Time.time + fireRate.stat.Value;

                Rpc_ShootDummyProjectile(targetSystem.syncedTargetPositions[i], firePoints[i].position);
            }
        }

        [ClientRpc]
        private void Rpc_ShootDummyProjectile(Vector3 targetPosition, Vector3 FirePointPosition)
        {
            GameObject projectile = Instantiate(projectilePrefab, FirePointPosition, Quaternion.identity);
            var bulletComponent = projectile.GetComponent<TurretProjectile>();
            bulletComponent.Setup(targetPosition, 0, projectileSpeed.stat.Value);
        }
    }
}
