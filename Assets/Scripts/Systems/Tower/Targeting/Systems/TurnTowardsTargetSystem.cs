using TDGame.Systems.Tower.Targeting.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TDGame.Systems.Tower.Targeting.Systems
{
    public partial struct TurnTowardsTargetSystem : ISystem
    {
        private ComponentLookup<LocalTransform> transformLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            transformLookup = state.GetComponentLookup<LocalTransform>(true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            transformLookup.Update(ref state);

            foreach (var (transform, turnTowardsTarget, targetBuffer) in SystemAPI
                         .Query<RefRO<LocalTransform>, RefRO<TurnTowardsTarget>, DynamicBuffer<TargetBufferElement>>())
            {
                if (targetBuffer.Length <= 0)
                    continue;
                
                var direction = transformLookup[targetBuffer[0]].Position - transform.ValueRO.Position;
                direction.y = 0;

                var newTransform = transformLookup[turnTowardsTarget.ValueRO.TurnPoint].WithRotation(math.nlerp(
                    transformLookup[turnTowardsTarget.ValueRO.TurnPoint].Rotation, quaternion.LookRotation(direction, math.up()),
                    SystemAPI.Time.DeltaTime * turnTowardsTarget.ValueRO.TurnSpeed));
                
                SystemAPI.SetComponent(turnTowardsTarget.ValueRO.TurnPoint, newTransform);
            }
        }
    }
}