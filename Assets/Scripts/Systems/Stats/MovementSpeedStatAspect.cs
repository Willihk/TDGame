using TDGame.Systems.Buff;
using TDGame.Systems.Buff.Implementations.Movement;
using TDGame.Systems.Stats.Implementations;
using Unity.Entities;

namespace TDGame.Systems.Stats
{
    public readonly partial struct MovementSpeedStatAspect : IAspect
    {
        [Optional]
        public readonly RefRO<BaseMovementSpeedStat> baseStat;

        public readonly RefRO<MovementSpeedBuff> buff;

        [Optional]
        public readonly RefRW<FinalMovementSpeedStat> finalStat;
    }
}