using TDGame.Systems.Buff.Implementations.Movement;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace TDGame.Systems.Buff.Systems
{
    public partial class BuffDurationSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem commandBufferSystem;

        protected override void OnUpdate()
        {
            commandBufferSystem = World.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
            ScheduleBuffDurationJob<MovementSpeedBuff>();
        }


        void ScheduleBuffDurationJob<TBuff>()
            where TBuff : unmanaged, IComponentData, IBaseBuff
        {
            var query = GetEntityQuery(ComponentType.ReadWrite<TBuff>());
            var handle = new CalcStats<TBuff>
            {
                BuffStateHandle = GetComponentTypeHandle<TBuff>(),
                EntityTypeHandle = GetEntityTypeHandle(),
                CommandBuffer = commandBufferSystem.CreateCommandBuffer()
            }.Schedule(query, Dependency);

            commandBufferSystem.AddJobHandleForProducer(handle);
            Dependency = JobHandle.CombineDependencies(Dependency, handle);
        }


        [BurstCompile]
        partial struct CalcStats<TBuff> : IJobChunk
            where TBuff : unmanaged, IComponentData, IBaseBuff
        {
            public EntityCommandBuffer CommandBuffer;

            public ComponentTypeHandle<TBuff> BuffStateHandle;

            public EntityTypeHandle EntityTypeHandle;

            [ReadOnly]
            public int DeltaTime;

            [BurstCompile]
            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask,
                in v128 chunkEnabledMask)
            {
                var enumerator = new ChunkEntityEnumerator(useEnabledMask, chunkEnabledMask, chunk.Count);
                var buffStats = chunk.GetNativeArray(ref BuffStateHandle);

                var entities = chunk.GetNativeArray(EntityTypeHandle);

                while (enumerator.NextEntityIndex(out int i))
                {
                    var buff = buffStats[i];
                    buff.Duration -= DeltaTime;
                    buffStats[i] = buff;
                    
                    if (buff.Duration <= 0)
                    {
                        CommandBuffer.RemoveComponent<TBuff>(entities[i]);
                    }
                }
            }
        }
    }
}