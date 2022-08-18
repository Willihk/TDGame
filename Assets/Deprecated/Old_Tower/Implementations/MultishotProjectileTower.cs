using System;
using Mirror;
using TDGame.Systems.Projectiles;
using TDGame.Systems.Stats;
using TDGame.Systems.Targeting.Implementations;
using TDGame.Systems.Tower.Base;
using UnityEngine;

namespace TDGame.Systems.Tower.Implementations
{
    [Obsolete]
    public class MultishotProjectileTower : BaseNetworkedTower
    {
        [SerializeField]
        protected SingleTargetSystem targetSystem;

        [SerializeField]
        protected GameObject projectilePrefab;

        [Header("Stats")]
        [Space(10)]
        [SerializeField]
        protected StatWrapper hitDamage;

        [SerializeField]
        protected StatWrapper fireRate;

        [SerializeField]
        protected int projectileCount;

        [SerializeField]
        protected float projectileAngle;

        [SerializeField]
        protected StatWrapper projectileSpeed;

        [Header("Visual")]
        [Space(10)]
        [SerializeField]
        protected Transform firePoint;

        private float nextFire;

        private void Update()
        {
            if (targetSystem.target != null && isServer)
            {
                if (nextFire < Time.time)
                {
                    ShootProjectiles();
                }
            }
        }

        private void ShootProjectiles()
        {
            nextFire = Time.time + fireRate.stat.Value;

            for (int i = 0; i < projectileCount; i++)
            {
                GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
                TurretProjectile projectileComponent = projectile.GetComponent<TurretProjectile>();

                Vector3 newPosition = CalculatePosition((projectileAngle / -2f) + (projectileAngle / (projectileCount - 1f) * i));
                //Vector3 newPosition = targetSystem.clientTargetPosition;

                projectileComponent.Setup(newPosition, hitDamage.stat.Value, projectileSpeed.stat.Value);

                Rpc_ShootDummyProjectile(newPosition);
            }
        }

        [ClientRpc]
        private void Rpc_ShootDummyProjectile(Vector3 position)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            var projectileComponent = projectile.GetComponent<TurretProjectile>();
            projectileComponent.Setup(position, 0, projectileSpeed.stat.Value);
        }

        private Vector3 CalculatePosition(float angle)
        {
            float angle2 = Mathf.Atan2(firePoint.position.y - targetSystem.clientTargetPosition.y, firePoint.position.x - targetSystem.clientTargetPosition.x) * 180 / Mathf.PI;

            float distance = Vector3.Distance(firePoint.position, targetSystem.clientTargetPosition);

            float num1 = Mathf.Cos(angle2 - angle) * distance;
            float num2 = Mathf.Sin(angle2 - angle) * distance;

            return new Vector3(num1 + firePoint.position.x, targetSystem.clientTargetPosition.y, num2 + firePoint.position.z);
        }
    }
}
