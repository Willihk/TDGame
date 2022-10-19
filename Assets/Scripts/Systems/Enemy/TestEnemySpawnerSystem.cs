using TDGame.PrefabManagement;
using TDGame.Systems.Enemy.Components.Spawning;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace TDGame.Systems.Enemy
{
    [BurstCompile]
    public partial struct TestEnemySpawnerSystem : ISystem
    {
        public double delay;

        private double nextSpawn;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            delay = 0.5f;
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (nextSpawn > SystemAPI.Time.ElapsedTime)
            {
                return;
            }
            nextSpawn = SystemAPI.Time.ElapsedTime + delay;
            
            var entity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponent<SpawnEnemy>(entity);
            
            var singletonEntity = SystemAPI.GetSingletonEntity<PrefabManagerTag>();
            var prefabManager = SystemAPI.GetBuffer<PrefabElement>(singletonEntity);
            state.EntityManager.SetComponentData(entity, new SpawnEnemy() { prefab = prefabManager[0].GUID });
        }
    }
}