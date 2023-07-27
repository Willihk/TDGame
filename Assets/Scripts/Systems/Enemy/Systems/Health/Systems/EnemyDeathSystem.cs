using TDGame.Systems.Economy;
using TDGame.Systems.Enemy.Systems.Health.Components;
using Unity.Burst;
using Unity.Collections;
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

            var goldReward = new NativeReference<int>(Allocator.TempJob);

            new DeathJob() { Gold = goldReward, CommandBuffer = ecb }.Run();
            
            if (goldReward.Value > 0)
                PlayerEconomyManager.Instance.AddCurrencyToAllPlayers(goldReward.Value);

            commandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        [BurstCompile]
        private partial struct DeathJob : IJobEntity
        {
            public NativeReference<int> Gold;
            public EntityCommandBuffer.ParallelWriter CommandBuffer;

            [BurstCompile]
            void Execute(Entity entity, [ChunkIndexInQuery] int sort, in EnemyHealthData healthData)
            {
                if (healthData.Health > 0)
                    return;

                Gold.Value += 3;
                // HealthBarUIPool.Instance.ReturnSlider(uiData.Slider);
                CommandBuffer.DestroyEntity(sort, entity);
            }
        }
    }
}