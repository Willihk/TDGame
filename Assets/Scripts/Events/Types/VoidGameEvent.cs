using System;
using TDGame.Events.Base;
using UnityEngine;

namespace TDGame.Events.Types
{

[CreateAssetMenu(fileName = "GameEvent", menuName = "GameEvents/VoidEvent")]
[Serializable]
public class VoidGameEvent : GameEvent
{
}

}