using System;
using TDGame.Events.Base;
using UnityEngine;

namespace TDGame.Events.Types
{
    [CreateAssetMenu(fileName = "GameEvent", menuName = "GameEvents/FloatEvent")]
    [Serializable]
    public class FloatGameEvent : GameEvent<float>
    {
    }
}