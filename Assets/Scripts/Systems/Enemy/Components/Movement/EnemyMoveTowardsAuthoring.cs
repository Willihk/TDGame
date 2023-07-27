using Unity.Entities;
using UnityEngine;

namespace TDGame.Systems.Enemy.Components.Movement
{
    public struct EnemyMoveTowards : IComponentData
    {
        public int waypointIndex;
    }
    
    public class EnemyMoveTowardsAuthoring : MonoBehaviour
    {
    }
    public class EnemyMoveTowardsBaker : Baker<EnemyMoveTowardsAuthoring>
    {
        public override void Bake(EnemyMoveTowardsAuthoring authoring)
        {
            AddComponent(new EnemyMoveTowards{waypointIndex = 0});
        }
    }
}