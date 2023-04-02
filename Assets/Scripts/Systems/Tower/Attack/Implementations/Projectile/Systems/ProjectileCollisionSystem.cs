using TDGame.Systems.Enemy.Components;
using TDGame.Systems.Enemy.Systems.Health.Components;
using TDGame.Systems.Tower.Attack.Implementations.Projectile.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace TDGame.Systems.Tower.Attack.Implementations.Projectile.Systems
{
    public partial class ProjectileCollisionSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem bufferSystem;
        private EntityQuery enemyQuery;

        private NativeQueue<Collision> collisions;

        protected override void OnCreate()
        {
            bufferSystem = World.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
            enemyQuery = GetEntityQuery(ComponentType.ReadOnly<EnemyTag>(), ComponentType.ReadOnly<LocalToWorld>(),
                ComponentType.ReadWrite<EnemyHealthData>());
            collisions = new NativeQueue<Collision>(Allocator.Persistent);
        }

        protected override void OnDestroy()
        {
            collisions.Dispose();
        }

        protected override void OnUpdate()
        {
            var enemyEntities = enemyQuery.ToEntityListAsync(Allocator.TempJob, out var enemyEntityHandle);
            var translations = GetComponentLookup<LocalToWorld>(true);

            var ecb = bufferSystem.CreateCommandBuffer();
            var handle = JobHandle.CombineDependencies(Dependency, enemyEntityHandle);
            
            new CollisionJob
            {
                AllTranslations = translations,
                EnemyEntities = enemyEntities.AsDeferredJobArray(),
                Collisions = collisions.AsParallelWriter()
            }.ScheduleParallel(handle).Complete();

            enemyEntities.Dispose();
            
            // need to be local variables to compile
            var queue = collisions;
            var manager = EntityManager;
            
            Job.WithCode(() =>
            {
                while (queue.TryDequeue(out var collision))
                {
                   var healthData = manager.GetComponentData<EnemyHealthData>(collision.EnemyEntity);
                   var projectileDamage = manager.GetComponentData<ProjectileDamage>(collision.ProjectileEntity);

                   healthData.Health -= projectileDamage.Value;
                   
                   manager.SetComponentData(collision.EnemyEntity, healthData);
                   ecb.DestroyEntity(collision.ProjectileEntity);
                }
            }).Run();
        }

        struct Collision
        {
            public Entity ProjectileEntity;
            public Entity EnemyEntity;
        }
        
        [BurstCompile]
        partial struct CollisionJob : IJobEntity
        {
            [ReadOnly]
            public NativeArray<Entity> EnemyEntities;

            [ReadOnly]
            public ComponentLookup<LocalToWorld> AllTranslations;

            [WriteOnly]
            public NativeQueue<Collision>.ParallelWriter Collisions;

            void Execute(Entity entity, in ProjectileDamage damage, in LocalToWorld translation,
                in ProjectileRadiusCollider radiusCollider)
            {
                var closestEntity = Entity.Null;
                float closestDistance = float.MaxValue;

                for (int i = 0; i < EnemyEntities.Length; i++)
                {
                    float distance = math.distance(AllTranslations[EnemyEntities[i]].Position, translation.Position);

                    if (distance < closestDistance && distance < radiusCollider.Value)
                    {
                        closestEntity = EnemyEntities[i];
                        closestDistance = distance;
                    }
                }

                if (closestEntity != Entity.Null)
                {
                    Collisions.Enqueue(new Collision(){ProjectileEntity = entity, EnemyEntity = closestEntity});
                }
            }
        }
    }
}