﻿using Unity.Entities;

namespace TDGame.Systems.Stats.Implementations.Damage
{
    public struct BaseDamageStat : IComponentData, IBaseStat
    {
        public float value;

        public float Value
        {
            readonly get => value;
            set => this.value = value;
        }
    }
}