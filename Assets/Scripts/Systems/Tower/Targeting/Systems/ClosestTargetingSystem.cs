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

            var handle = new TargetJob
            {
                Quadtree = tree,
            }.ScheduleParallel(JobHandle.CombineDependencies(
                state.WorldUnmanaged.GetExistingSystemState<EnemyTreeSystem>().Dependency, state.Dependency));

            state.Dependency = JobHandle.CombineDependencies(state.Dependency, handle);
        }

        [BurstCompile]
        private partial struct TargetJob : IJobEntity
        {
            [ReadOnly]
            public NativeQuadtree<Entity> Quadtree;

            [BurstCompile]
            void Execute(in FinalRangeStat range, in LocalTransform transform,
                ref DynamicBuffer<TargetBufferElement> buffer, in RequestEnemyTargets request)
            {
                var nearest = new NativeQueue<Entity>(Allocator.Temp);
                var visitor = new NativeQuadtreeExtensions.QuadtreeNNearestRangeVisitor<Entity>()
                {
                    Nearest = nearest,
                    Count = request.Count
                };

                var query = new NativeQuadtree<Entity>.NearestNeighbourQuery(Allocator.Temp);
                query.Nearest(ref Quadtree, transform.Position.xz, range.Value,
                    ref visitor, default(NativeQuadtreeExtensions.AABBDistanceSquaredProvider<Entity>));

                buffer.Clear();
                while (nearest.TryDequeue(out var enemy))
                {
                    buffer.Add(enemy);
                }
            }
        }
    }
}