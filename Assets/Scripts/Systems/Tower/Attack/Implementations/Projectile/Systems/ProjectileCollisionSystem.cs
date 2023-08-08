using NativeTrees;
using TDGame.Systems.Enemy.Systems.Health.Components;
using TDGame.Systems.Grid.SpatialTree;
using TDGame.Systems.Stats.Implementations.Damage;
using TDGame.Systems.Tower.Attack.Implementations.Projectile.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

namespace TDGame.Systems.Tower.Attack.Implementations.Projectile.Systems
{
    [UpdateAfter(typeof(EnemyTreeSystem))]
    public partial class ProjectileCollisionSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem bufferSystem;

        private ComponentLookup<EnemyHealthData> healthLookup;

        private NativeQueue<Collision> collisions;

        struct Collision
        {
            public int Damage;
            public Entity Entity;
        }

        protected override void OnCreate()
        {
            RequireForUpdate<MapDetailsSingleton>();

            bufferSystem = World.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();

            healthLookup = GetComponentLookup<EnemyHealthData>(false);

            collisions = new NativeQueue<Collision>(Allocator.Persistent);
        }

        protected override void OnDestroy()
        {
            collisions.Dispose();
        }

        protected override void OnUpdate()
        {
            var treeHandle = World.Unmanaged.GetExistingUnmanagedSystem<EnemyTreeSystem>();

            var treeSystem = World.Unmanaged.GetUnsafeSystemRef<EnemyTreeSystem>(treeHandle);
            if (!treeSystem.GetTree(out var tree))
                return;

            healthLookup.Update(this);


            var ecb = bufferSystem.CreateCommandBuffer();

            collisions.Clear();
            var handle = new CollisionJob
            {
                CommandBuffer = ecb.AsParallelWriter(),
                HealthLookup = healthLookup,
                Quadtree = tree,
                CollisionQueue = collisions.AsParallelWriter()
            }.ScheduleParallel(JobHandle.CombineDependencies(Dependency,
                World.Unmanaged.GetExistingSystemState<EnemyTreeSystem>().Dependency));
            Dependency = JobHandle.CombineDependencies(Dependency, handle);

            bufferSystem.AddJobHandleForProducer(Dependency);

            handle = new DealDamageJob
            {
                CollisionQueue = collisions,
                HealthLookup = healthLookup
            }.Schedule(Dependency);
            Dependency = JobHandle.CombineDependencies(Dependency, handle);
        }

        private struct DealDamageJob : IJob
        {
            public NativeQueue<Collision> CollisionQueue;

            public ComponentLookup<EnemyHealthData> HealthLookup;

            public void Execute()
            {
                while (CollisionQueue.TryDequeue(out var collision))
                {
                    if (!HealthLookup.HasComponent(collision.Entity))
                        continue;
                    
                    var healthData = HealthLookup[collision.Entity];
                    healthData.Health -= collision.Damage;
                    HealthLookup[collision.Entity] = healthData;
                }
            }
        }
        
        [BurstCompile]
        partial struct CollisionJob : IJobEntity
        {
            [WriteOnly]
            public NativeQueue<Collision>.ParallelWriter CollisionQueue;
            
            [ReadOnly]
            public ComponentLookup<EnemyHealthData> HealthLookup;

            [ReadOnly]
            public NativeQuadtree<Entity> Quadtree;

            public EntityCommandBuffer.ParallelWriter CommandBuffer;

            void Execute(Entity entity, [ChunkIndexInQuery] int sort, in FinalDamageStat damage,
                in LocalToWorld translation,
                in ProjectileRadiusCollider radiusCollider)
            {
                var nearest = new NativeQueue<Entity>(Allocator.Temp);
                var visitor = new NativeQuadtreeExtensions.QuadtreeNNearestRangeVisitor<Entity>
                {
                    Nearest = nearest,
                    Count = 1
                };

                var query = new NativeQuadtree<Entity>.NearestNeighbourQuery(Allocator.Temp);
                query.Nearest(ref Quadtree, translation.Position.xz, radiusCollider.Value,
                    ref visitor, default(NativeQuadtreeExtensions.AABBDistanceSquaredProvider<Entity>));

                while (nearest.TryDequeue(out var enemy))
                {
                    if (!HealthLookup.HasComponent(enemy))
                        continue;
                    
                    // var healthData = HealthLookup[enemy];
                    // healthData.Health -= (int)damage.Value;
                    // HealthLookup[enemy] = healthData;
                    
                    CollisionQueue.Enqueue(new Collision {Damage = (int)damage.Value, Entity = enemy});
                    
                    CommandBuffer.DestroyEntity(sort, entity);
                }
            }
        }
    }
}