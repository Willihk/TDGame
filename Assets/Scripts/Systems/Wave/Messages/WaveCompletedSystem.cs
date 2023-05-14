using TDGame.Events;
using TDGame.Systems.Enemy.Components;
using Unity.Collections;
using Unity.Entities;

namespace TDGame.Systems.Wave.Messages
{
    public partial class WaveCompletedSystem : SystemBase
    {
        private EntityQuery spawnEntryQuery;
        private EntityQuery enemyQuery;

        protected override void OnCreate()
        {
            var builder = new EntityQueryBuilder(Allocator.Temp);
            builder.WithAll<WaveSpawnEntry>();
            spawnEntryQuery = builder.Build(this);
            
            builder = new EntityQueryBuilder(Allocator.Temp);
            builder.WithAll<EnemyTag>();
            enemyQuery = builder.Build(this);
        }

        protected override void OnUpdate()
        {
            if (!SystemAPI.TryGetSingleton(out WaveGlobalState waveGlobalState) || waveGlobalState.State == WaveState.Idle)
                return;

            if (!spawnEntryQuery.IsEmpty || !enemyQuery.IsEmpty)
                return;
            
            SystemAPI.SetSingleton(waveGlobalState);
            waveGlobalState.State = WaveState.Idle;
            WaveManager.Instance.WaveCompleted();
        }
    }
}