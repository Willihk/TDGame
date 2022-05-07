using TDGame.Systems.Tower.Attack.Implementations.Projectile.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TDGame.Systems.Tower.Attack.Implementations.Projectile.Systems
{
    public partial class ProjectileMovementSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;
            Entities.ForEach((ref Translation translation, in ProjectileMovementSpeed speed, in ProjectileMovementTarget target) =>
            {
                var direction = math.normalize(target.Value - translation.Value);
                direction.y = 0;
                translation.Value += direction * speed.Value * deltaTime;
            }).ScheduleParallel();
        }
    }
}