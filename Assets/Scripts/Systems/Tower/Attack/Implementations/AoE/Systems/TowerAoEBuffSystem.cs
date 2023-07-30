using NativeTrees;
using TDGame.Systems.Buff.Implementations.Movement;
using TDGame.Systems.Enemy.Systems.Health.Components;
using TDGame.Systems.Grid.SpatialTree;
using TDGame.Systems.Stats.Implementations.Range;
using TDGame.Systems.Tower.Attack.Implementations.AoE.Components;
using TDGame.Systems.Tower.Attack.Windup.Components;
using TDGame.Systems.Tower.Targeting.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

namespace TDGame.Systems.Tower.Attack.Implementations.AoE.Systems
{
    [BurstCompile]
    public partial struct TowerAoEBuffSystem : ISystem
    {
        private ComponentLookup<MovementSpeedBuff> buffLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<MapDetailsSingleton>();
            buffLookup = state.GetComponentLookup<MovementSpeedBuff>(true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            buffLookup.Update(ref state);

            var treeHandle = state.WorldUnmanaged.GetExistingUnmanagedSystem<EnemyTreeSystem>();

            var treeSystem = state.WorldUnmanaged.GetUnsafeSystemRef<EnemyTreeSystem>(treeHandle);
            if (!treeSystem.GetTree(out var tree))
                return;

            var commandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

            var handle = new DamageJob
            {
                BuffLookup = buffLookup,
                Quadtree = tree,
                CommandBuffer = commandBuffer
            }.ScheduleParallel(state.WorldUnmanaged.GetExistingSystemState<EnemyTreeSystem>().Dependency);

            state.Dependency = JobHandle.CombineDependencies(state.Dependency, handle);
        }

        [BurstCompile]
        partial struct DamageJob : IJobEntity
        {
            [ReadOnly]
            public ComponentLookup<MovementSpeedBuff> BuffLookup;

            [ReadOnly]
            public NativeQuadtree<Entity> Quadtree;

            public EntityCommandBuffer.ParallelWriter CommandBuffer;

            [BurstCompile]
            void Execute([ChunkIndexInQuery] int sort, ref BasicWindup windup, in LocalTransform transform,
                in FinalRangeStat range, in MovementSpeedBuff buff)
            {
                if (windup.Remainingtime > 0)
                    return;

                windup.Remainingtime = windup.WindupTime;

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
                    if (!BuffLookup.HasComponent(enemy))
                        CommandBuffer.AddComponent<MovementSpeedBuff>(sort, enemy);

                    CommandBuffer.SetComponent(sort, enemy, buff);
                }
            }
        }

        struct NearestVisitor : IQuadtreeNearestVisitor<Entity>
        {
            public NativeQueue<Entity> Nearest;

            public bool OnVist(Entity obj)
            {
                Nearest.Enqueue(obj);

                return true;
            }
        }
    }
}