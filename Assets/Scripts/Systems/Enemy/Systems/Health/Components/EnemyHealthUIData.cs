using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.UI;

namespace TDGame.Systems.Enemy.Systems.Health.Components
{
    [GenerateAuthoringComponent]
    public class EnemyHealthUIData : IComponentData
    {
        public Slider Slider;
        public float3 Offset;
    }
}