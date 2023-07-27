using System;
using TDGame.Systems.Buff;
using TDGame.Systems.Buff.Implementations.Movement;
using TDGame.Systems.Stats.Implementations;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace TDGame.Systems.Stats
{
    public partial class CalculateStatsSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem commandBufferSystem;
        protected override void OnUpdate()
        {
            commandBufferSystem = World.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
          ScheduleCalcJob<BaseMovementSpeedStat, MovementSpeedBuff, FinalMovementSpeedStat>();
        }


        void ScheduleCalcJob<TBase, TBuff, TFinal>()
            where TBase : unmanaged, IComponentData, IBaseStat
            where TBuff : unmanaged, IComponentData, IBaseBuff
            where TFinal : unmanaged, IComponentData, IBaseStat
        {
            
            var query = GetEntityQuery(ComponentType.ReadOnly<TBase>());
            var handle =new CalcStats<TBase, TBuff, TFinal>
            {
                BaseStateHandle = GetComponentTypeHandle<TBase>(true),
                BuffStateHandle = GetComponentTypeHandle<TBuff>(true),
                FinalStateHandle = GetComponentTypeHandle<TFinal>(),
                EntityTypeHandle = GetEntityTypeHandle(),
                CommandBuffer = commandBufferSystem.CreateCommandBuffer()
            }.Schedule(query, Dependency);
            
            commandBufferSystem.AddJobHandleForProducer(handle);
            Dependency = JobHandle.CombineDependencies(Dependency, handle);

        }


        [BurstCompile]
        partial struct CalcStats<TBase, TBuff, TFinal> : IJobChunk
            where TBase : unmanaged, IComponentData, IBaseStat
            where TBuff : unmanaged, IComponentData, IBaseBuff
            where TFinal : unmanaged, IComponentData, IBaseStat
        {
            public EntityCommandBuffer CommandBuffer;

            [ReadOnly]
            public ComponentTypeHandle<TBase> BaseStateHandle;
            [ReadOnly]
            public ComponentTypeHandle<TBuff> BuffStateHandle;
            
            public ComponentTypeHandle<TFinal> FinalStateHandle;

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