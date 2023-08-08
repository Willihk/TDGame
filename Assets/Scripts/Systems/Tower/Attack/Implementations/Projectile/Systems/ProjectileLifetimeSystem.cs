using TDGame.Systems.Tower.Attack.Implementations.Projectile.Components;
using Unity.Entities;

namespace TDGame.Systems.Tower.Attack.Implementations.Projectile.Systems
{
    public partial class ProjectileLifetimeSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem bufferSystem;

        protected override void OnCreate()
        {
            bufferSystem = World.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = bufferSystem.CreateCommandBuffer().AsParallelWriter();
            float deltaTime = SystemAPI.Time.DeltaTime;
            
            Entities.ForEach((Entity entity, int entityInQueryIndex, ref ProjectileLifetime lifetime) =>
            {
                lifetime.Value -= deltaTime;
                if (lifetime.Value <= 0)
                    ecb.DestroyEntity(entityInQueryIndex, entity);
            }).ScheduleParallel();
            
            bufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}