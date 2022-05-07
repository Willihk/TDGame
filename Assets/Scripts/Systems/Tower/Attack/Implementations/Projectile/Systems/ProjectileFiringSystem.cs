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
        private EndSimulationEntityCommandBufferSystem commandBufferSystem;

        protected override void OnCreate()
        {
            commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = commandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            var allTranslations = GetComponentDataFromEntity<Translation>(true);

            Entities.WithReadOnly(allTranslations).ForEach((Entity e, int entityInQueryIndex, ref BasicWindup windup,
                in Translation translation, in ProjectilePrefab prefab,
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
                ecb.SetComponent(entityInQueryIndex, entity, new Translation { Value = translation.Value });

                var direction = enemyTranslation.Value - translation.Value;
                ecb.SetComponent(entityInQueryIndex, entity,
                    new Rotation() { Value = quaternion.LookRotation(direction, new float3(0, 1, 0)) });

                ecb.SetComponent(entityInQueryIndex, entity,
                    new ProjectileMovementTarget { Value = enemyTranslation.Value });
            }).ScheduleParallel();

            commandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}