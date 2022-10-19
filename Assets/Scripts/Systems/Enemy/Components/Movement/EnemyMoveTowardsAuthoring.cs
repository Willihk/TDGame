using Unity.Entities;
using UnityEngine;

namespace TDGame.Systems.Enemy.Components.Movement
{
    public struct EnemyMoveTowards : IComponentData
    {
        public float Speed;
        public int waypointIndex;
    }
    
    public class EnemyMoveTowardsAuthoring : MonoBehaviour
    {
        public float Speed;
    }
    public class EnemyMoveTowardsBaker : Baker<EnemyMoveTowardsAuthoring>
    {
        public override void Bake(EnemyMoveTowardsAuthoring authoring)
        {
            AddComponent(new EnemyMoveTowards{Speed = authoring.Speed, waypointIndex = 0});
        }
    }
}