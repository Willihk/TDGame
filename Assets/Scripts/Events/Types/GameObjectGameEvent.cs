using System;
using TDGame.Events.Base;
using UnityEngine;

namespace TDGame.Events.Types
{
    [CreateAssetMenu(fileName = "GameEvent", menuName = "GameEvents/GameObjectEvent")]
    [Serializable]
    public class GameObjectGameEvent : GameEvent<GameObject>
    {
    }
}
