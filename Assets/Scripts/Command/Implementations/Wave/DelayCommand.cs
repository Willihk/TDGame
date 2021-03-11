namespace TDGame.Command.Implementations.Wave
{
    public class DelayCommand : WaveCommand
    {
        private unsafe float* target;

        private readonly float amount;

        public unsafe DelayCommand(float* delayTarget, float delayAmount)
        {
                target = delayTarget;
                amount = delayAmount;
        }

        public override void Execute()
        {
            unsafe
            {
                *target = amount;
            }
        }
    }
}