using System;
using TDGame.Systems.Stats;

namespace TDGame.Systems.Enemy.DamageReceiver.Processes
{
    public static class ArmorProcess
    {
        public static float CalculateDamageTaken(float damage, Stat armorStat)
        {
            damage -= armorStat.Value;
            return Math.Max(damage, 0);
        }
    }
}