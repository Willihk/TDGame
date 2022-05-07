using Unity.Entities;

namespace TDGame.Systems.Tower.Attack.Windup.Components
{
    [GenerateAuthoringComponent]
    public struct BasicWindup : IComponentData
    {
        public float WindupTime;

        public float Remainingtime;
    }
}