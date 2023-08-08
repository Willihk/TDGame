using TDGame.Systems.Tower.Attack.Windup.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace TDGame.Systems.Tower.Attack.Windup.Systems
{
    public partial class BasicWindupSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            Entities.ForEach((ref BasicWindup windup) =>
            {
                if (windup.Remainingtime > 0)
                    windup.Remainingtime -= deltaTime;
                
            }).ScheduleParallel();
        }
    }
}
