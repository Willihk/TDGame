using System;
using TDGame.Events.Base;
using UnityEngine;

namespace TDGame.Events.Types
{
    [CreateAssetMenu(fileName = "GameEvent", menuName = "GameEvents/IntHashEvent")]
    [Serializable]
    public class IntHashGameEvent : GameEvent<int, Hash128>
    {
    }
}