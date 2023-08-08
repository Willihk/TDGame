using System;
using TDGame.Events.Base;
using UnityEngine;

namespace TDGame.Events.Types
{
    [CreateAssetMenu(fileName = "GameEvent", menuName = "GameEvents/UlongEvent")]
    [Serializable]
    public class UlongGameEvent : GameEvent<ulong>
    {
    }
}