using Mirror;
using TDGame.Systems.Projectiles;
using TDGame.Systems.Stats;
using TDGame.Systems.Targeting.Implementations;
using TDGame.Systems.Tower.Base;
using UnityEngine;

namespace TDGame.Systems.Tower.Implementations
{
    public class ProjectileTower : BaseNetworkedTower
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
        protected StatWrapper projectileSpeed;

        [Header("Visual")]
        [Space(10)]
        [SerializeField]
        protected Transform firePoint;

        private float nextFire;

        void ShootProjectile()
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            var bulletComponent = projectile.GetComponent<TurretProjectile>();
            bulletComponent.Setup(targetSystem.target.transform.position, hitDamage.stat.Value, projectileSpeed.stat.Value);
            nextFire = Time.time + fireRate.stat.Value;

            Rpc_ShootDummyProjectile(targetSystem.target.transform.position);
        }

        [ClientRpc]
        private void Rpc_ShootDummyProjectile(Vector3 position)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            var bulletComponent = projectile.GetComponent<TurretProjectile>();
            bulletComponent.Setup(position, 0, projectileSpeed.stat.Value);
        }

        private void Update()
        {
            if (targetSystem.target != null && isServer)
            {
                if (nextFire < Time.time)
                {
                    ShootProjectile();
                }
            }
        }
    }
}