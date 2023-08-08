using TDGame.Systems.Enemy.Components.Spawning;
using Unity.Entities;

namespace TDGame.Systems.Wave
{
    public partial class WaveSpawnSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem commandBufferSystem;

        protected override void OnCreate()
        {
            commandBufferSystem = World.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            if (!SystemAPI.TryGetSingleton(out WaveGlobalState waveGlobalState) || waveGlobalState.State == WaveState.Idle)
                return;

            waveGlobalState.WaveElapsedTime += SystemAPI.Time.DeltaTime;
            SystemAPI.SetSingleton(waveGlobalState);

            var commandBuffer = commandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            
            Entities.ForEach((Entity entity,  int entityInQueryIndex, in WaveSpawnEntry spawnEntry) =>
            {
                if (!(spawnEntry.Time <= waveGlobalState.WaveElapsedTime))
                    return;
                
                var newEntity = commandBuffer.CreateEntity(entityInQueryIndex);
                commandBuffer.AddComponent<SpawnEnemy>(entityInQueryIndex, newEntity);
                commandBuffer.SetComponent(entityInQueryIndex, newEntity, new SpawnEnemy {prefab = spawnEntry.Guid});
                    
                commandBuffer.DestroyEntity(entityInQueryIndex, entity);
            }).ScheduleParallel();
            
            
            commandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}