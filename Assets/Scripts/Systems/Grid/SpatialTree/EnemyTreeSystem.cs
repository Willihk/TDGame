using NativeTrees;
using TDGame.Systems.Enemy.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace TDGame.Systems.Grid.SpatialTree
{
    public partial struct EnemyTreeSystem : ISystem, ISystemStartStop
    {
        private NativeQuadtree<Entity> tree;

        private bool isReady;

        public bool GetTree(out NativeQuadtree<Entity> enemyTree)
        {
            enemyTree = tree;
            return isReady;
        }

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MapDetailsSingleton>();
        }

        public void OnStartRunning(ref SystemState state)
        {
            var details = SystemAPI.GetSingleton<MapDetailsSingleton>();

            tree = new NativeQuadtree<Entity>(new AABB2D(new float2(0, 0), new float2(details.Size.x, details.Size.y)),
                16, 8, Allocator.Persistent, 0);
            isReady = true;
        }

        public void OnStopRunning(ref SystemState state)
        {
            isReady = false;
            tree.Dispose();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            tree.Clear();

            var job = new GenerateTreeJob
            {
                Quadtree = tree
            }.Schedule(state.Dependency);
            
            state.Dependency = JobHandle.CombineDependencies(job, state.Dependency);
        }

        [BurstCompile]
        partial struct GenerateTreeJob : IJobEntity
        {
            public NativeQuadtree<Entity> Quadtree;

            [BurstCompile]
            void Execute(Entity entity, in LocalTransform transform, in EnemyTag _)
            {
                Quadtree.InsertPoint(entity, transform.Position.xz);
            }
        }
    }
}