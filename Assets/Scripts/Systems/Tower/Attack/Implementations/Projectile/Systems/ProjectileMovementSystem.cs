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
            float deltaTime = SystemAPI.Time.DeltaTime;
            Entities.ForEach((ref LocalTransform transform, in ProjectileMovementSpeed speed, in ProjectileMovementTarget target) =>
            {
                var direction = math.normalize(target.Value - transform.Position);
                direction.y = 0;
                transform.Position += direction * speed.Value * deltaTime;
            }).ScheduleParallel();
            
            Entities.ForEach((ref LocalTransform transform, in ProjectileMovementSpeed speed, in ProjectileMovementDirection moveDirection) =>
            {
                var direction = moveDirection.Value;
                direction.y = 0;
                transform.Position += direction * speed.Value * deltaTime;
            }).ScheduleParallel();
        }
    }
}