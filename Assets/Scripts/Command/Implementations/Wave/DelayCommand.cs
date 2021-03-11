using TDGame.Systems.Enemy.Wave;

namespace TDGame.Command.Implementations.Wave
{
    public class DelayCommand : WaveCommand
    {
        private EnemyWaveController waveController;

        private readonly float amount;

        public DelayCommand(EnemyWaveController waveController, float delayAmount)
        {
            this.waveController = waveController;
            amount = delayAmount;
        }

        public override void Execute()
        {
            waveController.delay = amount;
        }
    }
}