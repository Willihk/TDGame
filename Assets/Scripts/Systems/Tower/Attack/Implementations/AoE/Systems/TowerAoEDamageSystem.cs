using TDGame.Systems.Enemy.Components;
using TDGame.Systems.Enemy.Systems.Health.Components;
using TDGame.Systems.Tower.Attack.Implementations.AoE.Components;
using TDGame.Systems.Tower.Attack.Windup.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TDGame.Systems.Tower.Attack.Implementations.AoE.Systems
{
    [BurstCompile]
    public partial struct TowerAoEDamageSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var towerComponents in SystemAPI.Query<RefRW<BasicWindup>, RefRO<LocalTransform>, RefRO<AoERange>, RefRO<AoEDamage>>())
            {
                if (towerComponents.Item1.ValueRO.Remainingtime > 0)
                    continue;

                towerComponents.Item1.ValueRW.Remainingtime = towerComponents.Item1.ValueRO.WindupTime;
                
                foreach (var (enemyTransform, enemyHealth) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<EnemyHealthData>>())
                {
                    float distance = math.distance(towerComponents.Item2.ValueRO.Position,
                        enemyTransform.ValueRO.Position);

                    if (distance <= towerComponents.Item3.ValueRO.Range)
                    {
                        enemyHealth.ValueRW.Health -= towerComponents.Item4.ValueRO.Damage;
                    }
                }
            }
        }
    }
}