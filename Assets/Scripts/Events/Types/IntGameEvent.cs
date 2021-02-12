using System;
using UnityEngine;

namespace TDGame.Events
{
    [CreateAssetMenu(fileName = "GameEvent", menuName = "GameEvents/IntEvent")]
    [Serializable]
    public class IntGameEvent : GameEvent<int>
    {
    }
}