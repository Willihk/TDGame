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
            var allTranslations = GetComponentLookup<LocalToWorldTransform>(true);

            Entities.WithReadOnly(allTranslations).ForEach((Entity e, int entityInQueryIndex, ref BasicWindup windup,
                in LocalToWorldTransform transform, in ProjectilePrefab prefab,
                in DynamicBuffer<TargetBufferElement> targets) =>
            {
                if (windup.Remainingtime > 0 || targets.Length == 0)
                    return;

                if (!allTranslations.TryGetComponent(targets[0].Value, out var enemyTranslation))
                    return;

                windup.Remainingtime = windup.WindupTime;

                var entity = ecb.Instantiate(entityInQueryIndex, prefab.Value);
                ecb.RemoveComponent<Prefab>(entityInQueryIndex, entity);
                ecb.AddComponent<ProjectileMovementTarget>(entityInQueryIndex, entity);

                var direction = enemyTranslation.Value.Position - transform.Value.Position;
                
                var projectileTransform = new LocalToWorldTransform {Value = UniformScaleTransform.Identity};
                projectileTransform.Value.Position = transform.Value.Position;
                projectileTransform.Value.Rotation = quaternion.LookRotation(direction, new float3(0, 1, 0));

                ecb.SetComponent(entityInQueryIndex, entity, projectileTransform);

                ecb.SetComponent(entityInQueryIndex, entity,
                    new ProjectileMovementTarget { Value = enemyTranslation.Value.Position });
            }).ScheduleParallel();

            bufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}