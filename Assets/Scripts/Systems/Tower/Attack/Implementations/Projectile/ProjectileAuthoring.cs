using Sirenix.OdinInspector;
using TDGame.Systems.Stats.Implementations.Damage;
using TDGame.Systems.Stats.Implementations.Movement;
using TDGame.Systems.Tower.Attack.Implementations.Projectile.Components;
using Unity.Entities;
using UnityEngine;

namespace TDGame.Systems.Tower.Attack.Implementations.Projectile
{
    public class ProjectileAuthoring : MonoBehaviour
    {
        [BoxGroup("Projectile Settings")]
        public float Speed;

        [BoxGroup("Projectile Settings")]
        public float ColliderRadius = 1;

        [BoxGroup("Projectile Settings")]
        public float Damage;

        [BoxGroup("Projectile Settings")]
        public float LifeTime;

        public class ProjectileAuthoringBaker : Baker<ProjectileAuthoring>
        {
            public override void Bake(ProjectileAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new BaseMovementSpeedStat { Value = authoring.Speed });
                AddComponent(entity, new BaseDamageStat { Value = authoring.Damage });
                AddComponent(entity, new ProjectileLifetime { Value = authoring.LifeTime });
                AddComponent(entity, new ProjectileRadiusCollider { Value = authoring.ColliderRadius });
            }
        }
    }
}