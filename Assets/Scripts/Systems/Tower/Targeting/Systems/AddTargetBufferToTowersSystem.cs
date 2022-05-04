using TDGame.Systems.Tower.Targeting.Components;
using Unity.Entities;
using Unity.Burst;

namespace TDGame.Systems.Tower.Targeting.Systems
{
    public partial class AddTargetBufferToTowersSystem : SystemBase
    {

        private EndSimulationEntityCommandBufferSystem commandBufferSystem;

        protected override void OnCreate()
        {
            commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var CommandBuffer = commandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            Entities.WithAll<TowerTag, RequestEnemyTargetTag>().WithNone<TargetBufferElement>().ForEach((Entity entity, int entityInQueryIndex) =>
            {
                CommandBuffer.AddBuffer<TargetBufferElement>(entityInQueryIndex, entity);

            }).ScheduleParallel();

            commandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}