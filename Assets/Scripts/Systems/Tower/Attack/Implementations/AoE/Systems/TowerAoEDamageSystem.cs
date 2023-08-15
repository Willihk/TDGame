﻿using NativeTrees;
using TDGame.Systems.Enemy.Systems.Health.Components;
using TDGame.Systems.Grid.SpatialTree;
using TDGame.Systems.Stats.Implementations.Damage;
using TDGame.Systems.Stats.Implementations.Range;
using TDGame.Systems.Tower.Attack.Implementations.AoE.Components;
using TDGame.Systems.Tower.Attack.Windup.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace TDGame.Systems.Tower.Attack.Implementations.AoE.Systems
{
    [BurstCompile]
    [UpdateAfter(typeof(EnemyTreeSystem))]
    public partial struct TowerAoEDamageSystem : ISystem
    {
        private ComponentLookup<EnemyHealthData> healthLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<MapDetailsSingleton>();
            healthLookup = state.GetComponentLookup<EnemyHealthData>(true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            healthLookup.Update(ref state);

            var treeHandle = state.WorldUnmanaged.GetExistingUnmanagedSystem<EnemyTreeSystem>();

            var treeSystem = state.WorldUnmanaged.GetUnsafeSystemRef<EnemyTreeSystem>(treeHandle);
            if (!treeSystem.GetTree(out var tree))
                return;

            var commandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

            var handle =new DamageJob
            {
                HealthLookup = healthLookup,
                Quadtree = tree,
                CommandBuffer = commandBuffer
            }.ScheduleParallel(JobHandle.CombineDependencies(state.Dependency, state.WorldUnmanaged.GetExistingSystemState<EnemyTreeSystem>().Dependency));
            
            state.Dependency = JobHandle.CombineDependencies(state.Dependency, handle);
        }

        [BurstCompile]
        partial struct DamageJob : IJobEntity
        {
            [ReadOnly]
            public ComponentLookup<EnemyHealthData> HealthLookup;
            [ReadOnly]
            public NativeQuadtree<Entity> Quadtree;

            public EntityCommandBuffer.ParallelWriter CommandBuffer;
            
            [BurstCompile]
            void Execute([ChunkIndexInQuery]int sort, ref BasicWindup windup, in LocalTransform transform, in FinalRangeStat range, in FinalDamageStat damage, in AoETowerTag tag)
            {
                if (windup.Remainingtime > 0)
                    return;

                windup.Remainingtime = windup.WindupTime;

                var nearest = new NativeQueue<Entity>(Allocator.Temp);
                var visitor = new NativeQuadtreeExtensions.QuadtreeNNearestRangeVisitor<Entity>
                {
                    Nearest = nearest,
                    Count = 1000
                };
                
                var query = new NativeQuadtree<Entity>.NearestNeighbourQuery(Allocator.Temp);
                query.Nearest(ref Quadtree, transform.Position.xz, range.Value,
                    ref visitor, default(NativeQuadtreeExtensions.AABBDistanceSquaredProvider<Entity>));

                while (nearest.TryDequeue(out var enemy))
                {
                    if (!HealthLookup.HasComponent(enemy))
                        continue;
                    
                    var enemyHealth = HealthLookup[enemy];
                    enemyHealth.Health -= (int)math.round(damage.Value);
                    CommandBuffer.SetComponent(sort, enemy, enemyHealth);
                }
            }
        }
    }
}