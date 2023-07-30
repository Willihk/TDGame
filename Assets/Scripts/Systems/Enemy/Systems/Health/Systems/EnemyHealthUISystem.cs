using TDGame.Systems.Enemy.Components;
using TDGame.Systems.Enemy.Systems.Health.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace TDGame.Systems.Enemy.Systems.Health.Systems
{
    public partial class EnemyHealthUISystem : SystemBase
    {
        protected override void OnUpdate()
        {
            if (HealthBarUIPool.Instance == null)
                return;
            
            // Add sliders
            Entities.WithAll<EnemyTag>().ForEach((EnemyHealthUIData uiData, in EnemyHealthData healthData) =>
            {
                if (uiData.Slider == null)
                {
                    uiData.Slider = HealthBarUIPool.Instance.GetNextSlider();
                }
            }).WithoutBurst().Run();

            
            // Update state
            Entities.ForEach((EnemyHealthUIData uiData, in EnemyHealthData healthData, in LocalTransform translation) =>
            {
                if (uiData.Slider == null)
                {
                    return;
                }
                uiData.Slider.transform.position = translation.Position + uiData.Offset;
                uiData.Slider.value = (float)healthData.Health / healthData.MaxHealth;
            }).WithoutBurst().Run();

            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
            // Cleanup
            Entities.WithNone<EnemyTag>().ForEach((Entity entity, EnemyHealthUIData uiData) =>
            {
                HealthBarUIPool.Instance.ReturnSlider(uiData.Slider);
                commandBuffer.RemoveComponent<EnemyHealthUIData>(entity);
                commandBuffer.DestroyEntity(entity);
            }).WithStructuralChanges().WithoutBurst().Run();
            commandBuffer.Playback(EntityManager);
        }
    }
}