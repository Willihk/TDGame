using Unity.Entities;
using UnityEngine;

namespace TDGame.Systems.Tower.Attack.Implementations.AoE.Components
{
    public struct AoEDamage : IComponentData
    {
        public int Damage;
    }

    public class AoEDamageAuthoring : MonoBehaviour
    {
        public int Damage;

        public class AoEDamageBaker : Baker<AoEDamageAuthoring>
        {
            public override void Bake(AoEDamageAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new AoEDamage { Damage = authoring.Damage });
            }
        }
    }
}