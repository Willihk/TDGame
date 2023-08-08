using System;
using TDGame.Events.Base;
using UnityEngine;
using Hash128 = Unity.Entities.Hash128;

namespace TDGame.Events.Types
{
    [CreateAssetMenu(fileName = "GameEvent", menuName = "GameEvents/IntHashEvent")]
    [Serializable]
    public class IntHashGameEvent : GameEvent<int, Hash128>
    {
    }
}