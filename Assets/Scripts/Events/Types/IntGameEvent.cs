using System;
using TDGame.Events.Base;
using UnityEngine;

namespace TDGame.Events.Types
{
    [CreateAssetMenu(fileName = "GameEvent", menuName = "GameEvents/IntEvent")]
    [Serializable]
    public class IntGameEvent : GameEvent<int>
    {
    }
}