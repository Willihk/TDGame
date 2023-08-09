using Unity.Entities;
using UnityEngine;

namespace TDGame.Systems.Tower.Targeting.Components
{
    public struct RequestEnemyTargets : IComponentData
    {
        public int Count;
    }
}