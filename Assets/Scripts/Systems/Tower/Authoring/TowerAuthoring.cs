using System;
using Sirenix.OdinInspector;
using TDGame.Systems.Buff.Implementations.Movement;
using TDGame.Systems.Stats.Implementations.Damage;
using TDGame.Systems.Stats.Implementations.Range;
using TDGame.Systems.Tower.Attack.Implementations.AoE.Components;
using TDGame.Systems.Tower.Attack.Implementations.Projectile.Components;
using TDGame.Systems.Tower.Attack.Windup.Components;
using TDGame.Systems.Tower.Targeting.Components;
using Unity.Entities;
using UnityEngine;

namespace TDGame.Systems.Tower.Authoring
{
    public class TowerAuthoring : MonoBehaviour
    {
        public enum TowerType
        {
            Projectile,
            Aoe
        }
        
        [EnumToggleButtons]
        public TowerType towerType;
 
        [BoxGroup]
        public int range;
        [BoxGroup]
        public float aps;

        
        [AssetsOnly]
        [BoxGroup("Projectile", VisibleIf = "@this.towerType == TowerType.Projectile")]
        public GameObject ProjectilePrefab;
        [BoxGroup("Projectile")]
        public Transform FiringPoint;


        [InfoBox("Leave empty if not in use")]
        [BoxGroup("Turn To Target",  VisibleIf = "@this.towerType == TowerType.Projectile")]
        public Transform TurnPoint;
        [BoxGroup("Turn To Target")]
        public float TurnSpeed;
        
        [BoxGroup("AoE Damage", VisibleIf = "@this.towerType == TowerType.Aoe")]
        public bool enableAoeDamage;

        [BoxGroup("AoE Damage")]
        public int areaDamage;
        
        
        [BoxGroup("AoE Buff",  VisibleIf = "@this.towerType == TowerType.Aoe")]
        public bool enableAoeBuff;

        [BoxGroup("AoE Buff/Speed")]
        public bool EnableSpeedBuff;
        [BoxGroup("AoE Buff/Speed"), EnableIf("EnableSpeedBuff")]
        public MovementSpeedBuff SpeedBuff;
        
        
        class Baker : Baker<TowerAuthoring>
        {
            public override void Bake(TowerAuthoring authoring)
            {
                var towerEntity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<TowerTag>(towerEntity);
                
                AddComponent(towerEntity, new BaseRangeStat {Value = authoring.range});
                AddComponent(towerEntity, new BasicWindup {Remainingtime = 1 / authoring.aps, WindupTime =1 / authoring.aps});

                switch (authoring.towerType)
                {
                    case TowerType.Projectile:
                        AddComponent(towerEntity, new RequestEnemyTargets {Count = 1});
                        
                        AddComponent(towerEntity, new ProjectileFiringPoint {firingPoint = GetEntity(authoring.FiringPoint, TransformUsageFlags.Dynamic)});
                        AddComponent(towerEntity, new ProjectilePrefab {Value = GetEntity(authoring.ProjectilePrefab, TransformUsageFlags.Dynamic)});

                        if (authoring.TurnPoint)
                        {
                            AddComponent(towerEntity, new TurnTowardsTarget {TurnPoint = GetEntity(authoring.TurnPoint, TransformUsageFlags.Dynamic), TurnSpeed = authoring.TurnSpeed});
                        }
                        break;
                    case TowerType.Aoe:
                        AddComponent<AoETowerTag>(towerEntity);
                        if (authoring.enableAoeDamage)
                        {
                            AddComponent(towerEntity, new BaseDamageStat {value = authoring.areaDamage});
                        }

                        if (authoring.enableAoeBuff)
                        {
                            AddComponent(towerEntity, authoring.SpeedBuff);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}