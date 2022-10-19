using TDGame.Systems.Enemy.Components;
using TDGame.Systems.Enemy.Systems.Health.Components;
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
            Entities.WithAll<EnemyTag>().ForEach((Entity entity, EnemyHealthUIData uiData, in EnemyHealthData healthData) =>
            {
                if (uiData.Slider == null)
                {
                    uiData.Slider = HealthBarUIPool.Instance.GetNextSlider();
                }
            }).WithoutBurst().Run();

            
            // Update state
            Entities.ForEach((Entity entity, EnemyHealthUIData uiData, in EnemyHealthData healthData, in LocalToWorldTransform translation) =>
            {
                if (uiData.Slider == null)
                {
                    return;
                }
                uiData.Slider.transform.position = translation.Value.Position + uiData.Offset;
                uiData.Slider.value = (float)healthData.Health / healthData.MaxHealth;
            }).WithoutBurst().Run();
            
            // // Cleanup -Done in death system
            // Entities.WithNone<EnemyTag>().ForEach((Entity entity, EnemyHealthUIData uiData) =>
            // {
            //     HealthBarUIPool.Instance.ReturnSlider(uiData.Slider);
            //     EntityManager.RemoveComponent<EnemyHealthUIData>(entity);
            //     EntityManager.DestroyEntity(entity);
            // }).WithStructuralChanges().WithoutBurst().Run();
        }
    }
}