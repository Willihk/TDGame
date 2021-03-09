using System;
using TDGame.Systems.Stats;

namespace TDGame.Systems.Enemy.DamageReceiver.Processes
{
    public static class ArmorProcess
    {
        /// <summary>
        /// Reduces damage by armor stat value
        /// </summary>
        /// <param name="damage">The damage taken</param>
        /// <param name="armorStat">The armor stat</param>
        /// <returns>The reduced damage value, which is never negative</returns>
        public static float CalculateDamageTaken(float damage, Stat armorStat)
        {
            damage -= armorStat.Value;
            return Math.Max(damage, 0);
        }
    }
}