using NativeTrees;
using TDGame.Systems.Grid.SpatialTree;
using TDGame.Systems.Stats.Implementations.Range;
using TDGame.Systems.Tower.Targeting.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

namespace TDGame.Systems.Tower.Targeting.Systems
{
    [BurstCompile]
    [UpdateAfter(typeof(EnemyTreeSystem))]
    public partial struct ClosestTargetingSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MapDetailsSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var treeHandle = state.WorldUnmanaged.GetExistingUnmanagedSystem<EnemyTreeSystem>();

            var treeSystem = state.WorldUnmanaged.GetUnsafeSystemRef<EnemyTreeSystem>(treeHandle);
            if (!treeSystem.GetTree(out var tree))
                return;

            var handle =new DamageJob
            {
                Quadtree = tree,
            }.ScheduleParallel(JobHandle.CombineDependencies(state.WorldUnmanaged.GetExistingSystemState<EnemyTreeSystem>().Dependency, state.Dependency));
            
            state.Dependency = JobHandle.CombineDependencies(state.Dependency, handle);
        }

        [BurstCompile]
        partial struct DamageJob : IJobEntity
        {
            [ReadOnly]
            public NativeQuadtree<Entity> Quadtree;

            [BurstCompile]
            void Execute(in FinalRangeStat range, in LocalTransform transform, ref DynamicBuffer<TargetBufferElement> buffer, in RequestEnemyTargetTag _)
            {
                var nearest = new NativeQueue<Entity>(Allocator.Temp);
                var visitor = new NearestVisitor()
                {
                    Nearest = nearest
                };
                
                var query = new NativeQuadtree<Entity>.NearestNeighbourQuery(Allocator.Temp);
                query.Nearest(ref Quadtree, transform.Position.xz, range.Value,
                    ref visitor, default(NativeQuadtreeExtensions.AABBDistanceSquaredProvider<Entity>));

                while (nearest.TryDequeue(out var enemy))
                {
                    if (buffer.Length == 1)
                    {
                        buffer[0] = enemy;
                    }
                    else
                    {
                        buffer.Add(enemy);
                    }
                }
            }
        }
        struct NearestVisitor : IQuadtreeNearestVisitor<Entity>
        {
            public NativeQueue<Entity> Nearest;

            public bool OnVist(Entity obj)
            {
                Nearest.Enqueue(obj);

                return false;
            }
        }
    }
}