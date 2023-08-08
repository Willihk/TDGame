using TDGame.Systems.Tower.Targeting.Components;
using Unity.Entities;
using UnityEngine;

namespace TDGame.Systems.Tower.Attack.Windup.Components
{
    public struct BasicWindup : IComponentData
    {
        public float WindupTime;

        public float Remainingtime;
    }
    
    public class BasicWindupAuthoring : MonoBehaviour
    {
        public float WindupTime;

        public float Remainingtime;
    }

    public class BasicWindupBaker : Baker<BasicWindupAuthoring>
    {
        public override void Bake(BasicWindupAuthoring authoring)
        {
            AddComponent(new BasicWindup { WindupTime = authoring.WindupTime, Remainingtime = authoring.Remainingtime});
        }
    }
}