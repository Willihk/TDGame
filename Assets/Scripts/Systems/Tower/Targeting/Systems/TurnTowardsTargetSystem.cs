using TDGame.Systems.Tower.Targeting.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TDGame.Systems.Tower.Targeting.Systems
{
    public partial struct TurnTowardsTargetSystem : ISystem
    {
        private ComponentLookup<LocalTransform> transformLookup;
        private EndSimulationEntityCommandBufferSystem.Singleton commandBufferSystem;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            transformLookup = state.GetComponentLookup<LocalTransform>(true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            transformLookup.Update(ref state);

            new TurnJob()
            {
                TransformLookup = transformLookup, DeltaTime = SystemAPI.Time.DeltaTime,
                CommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged)
            }.Schedule();
        }


        [BurstCompile]
        partial struct TurnJob : IJobEntity
        {
            [ReadOnly]
            public ComponentLookup<LocalTransform> TransformLookup;
            [ReadOnly]
            public float DeltaTime;
            
            public EntityCommandBuffer CommandBuffer;

            [BurstCompile]
            void Execute(in LocalTransform transform, in TurnTowardsTarget turnTowardsTarget,
                in DynamicBuffer<TargetBufferElement> targetBuffer)
            {
                if (targetBuffer.Length <= 0)
                    return;

                var direction = TransformLookup[targetBuffer[0]].Position - transform.Position;
                direction.y = 0;

                var newTransform = TransformLookup[turnTowardsTarget.TurnPoint].WithRotation(math.nlerp(
                    TransformLookup[turnTowardsTarget.TurnPoint].Rotation,
                    quaternion.LookRotation(direction, math.up()),
                    DeltaTime * turnTowardsTarget.TurnSpeed));

                CommandBuffer.SetComponent(turnTowardsTarget.TurnPoint, newTransform);
            }
        }
    }
}