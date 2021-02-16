using System;
using TDGame.Events.Base;
using UnityEngine;

namespace TDGame.Events.Types
{
    [CreateAssetMenu(fileName = "GameEvent", menuName = "GameEvents/StringEvent")]
    [Serializable]
    public class StringGameEvent : GameEvent<string>
    {
    }
}