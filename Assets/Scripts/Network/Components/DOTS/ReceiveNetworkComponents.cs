using System;
using TDGame.Network.Components.Interfaces;
using TDGame.Systems.Enemy.Components.Spawning;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace TDGame.Network.Components.DOTS
{
    public partial class ReceiveNetworkComponents : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem commandBufferSystem;

        protected override void OnCreate()
        {
            commandBufferSystem = World.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
        }


        public void ReceiveData(NativeArray<byte> data)
        {
            var job = new SpawnEnemyDeserializeJob
            {
                CommandBuffer = commandBufferSystem.CreateCommandBuffer(),
                Data = data
            };

            job.Run();
        }

        [BurstCompile]
        struct SpawnEnemyDeserializeJob : IJob
        {
            public EntityCommandBuffer CommandBuffer;

            [DeallocateOnJobCompletion]
            public NativeArray<byte> Data;

            public void Execute()
            {
                // chunk.GetDynamicComponentDataArrayReinterpret<SpawnEnemy>(SpawnEnemyHandle);

                Span<byte> span = Data.AsSpan().Slice(2);

                int i = 0;
                while (i < Data.Length)
                {
                    int size = SpawnEnemy.TDLength();
                    var slice = span.Slice(i, size);

                    var component = SpawnEnemy.TDDeserialize(ref slice);

                    var entity = CommandBuffer.CreateEntity();
                    CommandBuffer.AddComponent<SpawnEnemy>(entity);
                    CommandBuffer.SetComponent(entity, component);

                    i += size;
                }
            }
        }


        [BurstCompile]
        struct SerializeJob : IJobChunk
        {
            public EntityCommandBuffer.ParallelWriter CommandBuffer;

            public EntityTypeHandle EntityType;

            public DynamicComponentTypeHandle SpawnEnemyHandle;

            [ReadOnly]
            public NativeHashMap<int, ComponentType> TypesInQuery;

            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask,
                in v128 chunkEnabledMask)
            {
                // chunk.GetDynamicComponentDataArrayReinterpret<SpawnEnemy>(SpawnEnemyHandle);

                var enumerator = new ChunkEntityEnumerator(useEnabledMask, chunkEnabledMask, chunk.Count);
                while (enumerator.NextEntityIndex(out int i))
                {
                }
            }
        }
    }
}