using TDGame.Systems.Enemy.Systems.Health.Components;
using Unity.Entities;

namespace TDGame.Systems.Enemy.Systems.Health.Systems
{
    public partial class EnemyDeathSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem commandBufferSystem;

        protected override void OnCreate()
        {
            commandBufferSystem = World.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = commandBufferSystem.CreateCommandBuffer().AsParallelWriter();

            Entities.ForEach((Entity entity ,int entityInQueryIndex, in EnemyHealthData healthData) =>
            {
                if (healthData.Health <= 0)
                {
                    // HealthBarUIPool.Instance.ReturnSlider(uiData.Slider);
                    ecb.DestroyEntity(entityInQueryIndex, entity);
                }

            }).WithoutBurst().Run();
            
            commandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}