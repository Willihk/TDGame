using TDGame.Systems.Enemy.Components;
using TDGame.Systems.Tower.Attack.Implementations.Projectile.Components;
using TDGame.Systems.Tower.Attack.Windup.Components;
using TDGame.Systems.Tower.Targeting.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TDGame.Systems.Tower.Attack.Implementations.Projectile.Systems
{
    public partial class ProjectileFiringSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem bufferSystem;

        protected override void OnCreate()
        {
            bufferSystem = World.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = bufferSystem.CreateCommandBuffer().AsParallelWriter();
            var allTranslations = GetComponentLookup<LocalTransform>(true);
            var allWorldTranslations = GetComponentLookup<LocalToWorld>(true);

            Entities.WithReadOnly(allTranslations).WithReadOnly(allWorldTranslations).ForEach((Entity e,
                int entityInQueryIndex, ref BasicWindup windup,
                in LocalTransform transform, in ProjectilePrefab prefab, in ProjectileFiringPoint firePoint,
                in DynamicBuffer<TargetBufferElement> targets) =>
            {
                if (windup.Remainingtime > 0 || targets.Length == 0)
                    return;
                foreach (var target in targets)
                {
                    if (!allTranslations.TryGetComponent(target.Value, out var enemyTranslation))
                        return;

                    windup.Remainingtime = windup.WindupTime;

                    var entity = ecb.Instantiate(entityInQueryIndex, prefab.Value);
                    ecb.AddComponent<ProjectileMovementDirection>(entityInQueryIndex, entity);


                    var enemyTargetPosition = enemyTranslation.Position;
                    enemyTargetPosition.y = allWorldTranslations[firePoint.firingPoint].Position.y;

                    var direction = enemyTargetPosition - allWorldTranslations[firePoint.firingPoint].Position;

                    var projectileTransform = new LocalTransform
                    {
                        Position = allWorldTranslations[firePoint.firingPoint].Position,
                        Rotation = quaternion.LookRotation(direction, new float3(0, 1, 0)),
                        Scale = 1
                    };

                    ecb.SetComponent(entityInQueryIndex, entity, projectileTransform);

                    ecb.SetComponent(entityInQueryIndex, entity,
                        new ProjectileMovementDirection { Value = direction });

                    ecb.RemoveComponent<Prefab>(entityInQueryIndex, entity);
                }
            }).ScheduleParallel();

            bufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}