using NativeTrees;
using TDGame.Systems.Enemy.Systems.Health.Components;
using TDGame.Systems.Grid.SpatialTree;
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
            }.ScheduleParallel(state.WorldUnmanaged.GetExistingSystemState<EnemyTreeSystem>().Dependency);
            
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
            void Execute([ChunkIndexInQuery]int sort,ref BasicWindup windup, in LocalTransform transform, in AoERange range, in AoEDamage damage)
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
                query.Nearest(ref Quadtree, transform.Position.xz, range.Range,
                    ref visitor, default(NativeQuadtreeExtensions.AABBDistanceSquaredProvider<Entity>));

                while (nearest.TryDequeue(out var enemy))
                {
                    var enemyHealth = HealthLookup[enemy];
                    enemyHealth.Health -= damage.Damage;
                    CommandBuffer.SetComponent(sort, enemy, enemyHealth);
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