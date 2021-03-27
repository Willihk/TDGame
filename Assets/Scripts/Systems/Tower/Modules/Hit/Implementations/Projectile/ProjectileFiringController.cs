using Mirror;
using Sirenix.OdinInspector;
using TDGame.Systems.Projectiles;
using TDGame.Systems.Stats;
using TDGame.Systems.TargetAcquisition.Implementations;
using TDGame.Systems.Targeting.Implementations;
using TDGame.Systems.Tower.Modules.Hit.Base;
using UnityEngine;

namespace TDGame.Systems.Tower.Modules.Hit.Implementations.Projectile
{
    public class ProjectileFiringController : TowerFiringController
    {
        [TabGroup("Setup")]
        [SerializeField]
        protected SingleTargetSystem targetSystem;

        [TabGroup("Setup")]
        [SerializeField]
        protected NetworkedStatsController statsController;

        [TabGroup("Setup")]
        [SerializeField]
        protected GameObject projectilePrefab;

        [TabGroup("Setup")]
        [SerializeField]
        protected Transform firePoint;

        [TabGroup("Stats")]
        [SerializeField]
        protected string hitDamageStatName = "HitDamage";

        [TabGroup("Stats")]
        [SerializeField]
        protected string projectileSpeedStatName = "ProjectileSpeed";


        public override void Fire()
        {
            if (!isServer)
                return;

            var target = targetSystem.target;
            if (!target)
                return;

            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            var bulletComponent = projectile.GetComponent<TurretProjectile>();
            bulletComponent.Setup(targetSystem.clientTargetPosition,
                statsController.GetStatByName(hitDamageStatName).stat.Value,
                statsController.GetStatByName(projectileSpeedStatName).stat.Value);

            Rpc_ShootDummyProjectile(targetSystem.clientTargetPosition);
        }

        [ClientRpc]
        private void Rpc_ShootDummyProjectile(Vector3 position)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            var bulletComponent = projectile.GetComponent<TurretProjectile>();
            bulletComponent.Setup(position, 0, statsController.GetStatByName(projectileSpeedStatName).stat.Value);
        }
    }
}