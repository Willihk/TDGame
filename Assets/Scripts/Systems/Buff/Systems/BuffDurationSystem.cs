using Unity.Burst;
using Unity.Entities;

namespace TDGame.Systems.Buff.Systems
{
  public partial class BuffDurationSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem commandBufferSystem;
        protected override void OnUpdate()
        {
            commandBufferSystem = World.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
          ScheduleCalcJob<MovementSpeedBuff>();
        }


        void ScheduleCalcJob<TBuff>()
            where TBuff : unmanaged, IComponentData, IBaseBuff
        {
            
            var query = GetEntityQuery(ComponentType.ReadWrite<TBuff>());
            var handle =new CalcStats<TBuff>
            {
                BuffStateHandle = GetComponentTypeHandle<TBuff>(),
                EntityTypeHandle = GetEntityTypeHandle(),
                CommandBuffer = commandBufferSystem.CreateCommandBuffer()
            }.Schedule(query, Dependency);
            
            commandBufferSystem.AddJobHandleForProducer(handle);
            Dependency = JobHandle.CombineDependencies(Dependency, handle);

        }


        [BurstCompile]
        partial struct CalcStats< TBuff > : IJobChunk
            where TBuff : unmanaged, IComponentData, IBaseBuff
        {
            public EntityCommandBuffer CommandBuffer;

            public ComponentTypeHandle<TBuff> BuffStateHandle;

            public EntityTypeHandle EntityTypeHandle;
            
            [BurstCompile]
            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
            {
                var enumerator = new ChunkEntityEnumerator(useEnabledMask, chunkEnabledMask, chunk.Count);
                NativeArray<TFinal> finalStats = new ();
                NativeArray<TBuff> buffStats = new ();

                var entities = chunk.GetNativeArray(EntityTypeHandle);
                var baseStats = chunk.GetNativeArray(ref BaseStateHandle);

                bool hasFinalComponent = chunk.Has<TFinal>();
                bool hasBuffComponent = chunk.Has<TBuff>();
                if (hasFinalComponent)
                {
                    finalStats = chunk.GetNativeArray(ref FinalStateHandle);
                }
                if (hasBuffComponent)
                {
                    buffStats = chunk.GetNativeArray(ref BuffStateHandle);
                }
                
                while (enumerator.NextEntityIndex(out int i))
                {
                    var finalValue = baseStats[i].Value;
                    
                    if (hasBuffComponent)
                    {
                        switch (buffStats[i].ModifierType)
                        {
                            case StatModifierType.Flat:
                                finalValue += buffStats[i].Value * finalValue * buffStats[i].Stacks;
                                break;
                            case StatModifierType.PercentAdditive:
                                finalValue *= buffStats[i].Value * buffStats[i].Stacks;
                                break;
                            case StatModifierType.PercentMultiplier:
                                finalValue *= math.pow(buffStats[i].Value, buffStats[i].Stacks);
                                break;
                        }
                    }
                    
                    if (hasFinalComponent)
                    {
                        finalStats[i] = new TFinal { Value = finalValue };
                    }
                    else
                    {
                        CommandBuffer.AddComponent<TFinal>(entities[i]);
                        CommandBuffer.SetComponent(entities[i], new TFinal() {Value = finalValue});
                    }
                }
            }
        }
    }
}