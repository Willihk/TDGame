using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace TDGame.Systems.Enemy.Systems.Health.Components
{
    public class EnemyHealthUIData : IComponentData
    {
        public Slider Slider;
        public float3 Offset;
    }

    public class EnemyHealthUIDataAuthoring : MonoBehaviour
    {
        public Slider Slider;
        public float3 Offset;
    }

    public class EnemyHealthUIDataBaker : Baker<EnemyHealthUIDataAuthoring>
    {
        public override void Bake(EnemyHealthUIDataAuthoring authoring)
        {
            AddComponentObject(new EnemyHealthUIData { Slider = authoring.Slider, Offset = authoring.Offset });
        }
    }
}