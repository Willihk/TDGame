using System;
using TDGame.Network.Components.Interfaces;
using TDGame.Systems.Enemy.Components.Spawning;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;

namespace TDGame.Network.Components.DOTS
{
    public partial class SendNetworkComponents : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem commandBufferSystem;
        
        protected override void OnCreate()
        {
            commandBufferSystem = World.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
        }
        
        protected override void OnUpdate()
        {
            // NativeList<EntityArchetype> archetypes = new NativeList<EntityArchetype>(Allocator.Temp);
            //
            // EntityManager.GetAllArchetypes(archetypes);
            //
            // // var se = (ITDSerializable)componentTypes[0];
            // // var e = componentTypes[0].GetType();
            //
            // // var info = TypeManager.GetTypeInfo<SpawnEnemy>();
            // // var t = new ComponentType(typeof(SpawnEnemy));
            //
            // var comptypes = new NativeHashMap<int, ComponentType>(1, Allocator.Persistent);
            //
            // comptypes.Add(TypeManager.GetTypeInfo<SpawnEnemy>().TypeIndex.Value, typeof(SpawnEnemy));
            //
            // ComponentType spawnType = typeof(SpawnEnemy);
            //
            // var archetypesToCheck = new NativeList<int>();
            //
            // for (int i = 0; i < archetypes.Length; i++)
            // {
            //     var componentTypes = archetypes[0].GetComponentTypes();
            //
            //     for (int j = 0; j < componentTypes.Length; j++)
            //     {
            //         if (componentTypes[j] != spawnType)
            //             continue;
            //         
            //         archetypesToCheck.Add(i);
            //         break;
            //     }
            //
            //     componentTypes.Dispose();
            // }
            //
            //
            // var queries = new EntityQuery[archetypesToCheck.Length];
            // for (int i = 0; i < archetypesToCheck.Length; i++)
            // {
            //     var componentTypes = archetypes[archetypesToCheck[i]].GetComponentTypes();
            //
            //     queries[i] = GetEntityQuery(new EntityQueryDesc
            //     {
            //         All = componentTypes.ToArray()
            //     });
            //     componentTypes.Dispose();
            // }

            // GetDynamicComponentTypeHandle()

            if (!CustomNetworkManager.Instance.serverWrapper.isListening)
            {
                return;
            }
            
            var query  = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]{ typeof(SpawnEnemy), typeof(NetworkSend)}
            });

            int entityCount = query.CalculateEntityCount();

            if (entityCount == 0)
                return;
            
            NativeArray<byte> data = new NativeArray<byte>(entityCount * SpawnEnemy.TDLength() + 2*sizeof(short), Allocator.TempJob);

            var newData = data.Reinterpret<int>(sizeof(byte));
            newData[1] = SpawnEnemy.NETWORK_MESSAGE_ID;
            newData[0] = entityCount;
            
            
            var job = new SpawnEnemySerializeJob
            {
                CommandBuffer = commandBufferSystem.CreateCommandBuffer().AsParallelWriter(),
                EntityType = GetEntityTypeHandle(),
                SpawnEnemyHandle = GetComponentTypeHandle<SpawnEnemy>(),
                Output = data
            };
            

            job.Run(query);
            
            
            CustomNetworkManager.Instance.serverWrapper.SendToAllEntities(data.AsSpan());

            data.Dispose();
        }
        
        
        [BurstCompile]
        struct SpawnEnemySerializeJob : IJobChunk
        {
            public EntityCommandBuffer.ParallelWriter CommandBuffer;
            
            public EntityTypeHandle EntityType;

            public ComponentTypeHandle<SpawnEnemy> SpawnEnemyHandle;

            public NativeArray<byte> Output;


            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
            {
                // chunk.GetDynamicComponentDataArrayReinterpret<SpawnEnemy>(SpawnEnemyHandle);

                var spawnEnemies = chunk.GetNativeArray(SpawnEnemyHandle);

                var entities = chunk.GetNativeArray(EntityType);
                
                var enumerator = new ChunkEntityEnumerator(useEnabledMask, chunkEnabledMask, chunk.Count);

                Span<byte> span = Output.AsSpan().Slice(2);
                while (enumerator.NextEntityIndex(out int i))
                {
                    int size = SpawnEnemy.TDLength();
                    var slice = span.Slice(i * size, size);

                    SpawnEnemy.TDSerialize(spawnEnemies[i], ref slice);
                    CommandBuffer.DestroyEntity(unfilteredChunkIndex, entities[i]);
                    // spawnEnemies[i].TDSerialize(ref slice);
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

            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
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