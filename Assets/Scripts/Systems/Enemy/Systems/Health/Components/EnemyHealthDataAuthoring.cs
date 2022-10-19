using Unity.Entities;
using UnityEngine;

namespace TDGame.Systems.Enemy.Systems.Health.Components
{
    public struct EnemyHealthData : IComponentData
    {
        public int Health;
        public int MaxHealth;
    }
    public class EnemyHealthDataAuthoring : MonoBehaviour
    {
        public int Health;
        public int MaxHealth;
    }
    public class EnemyHealthDataBaker : Baker<EnemyHealthDataAuthoring>
    {
        public override void Bake(EnemyHealthDataAuthoring authoring)
        {
            AddComponent(new EnemyHealthData{Health = authoring.Health, MaxHealth = authoring.MaxHealth});
        }
    }
}