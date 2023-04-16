using System;
using TDGame.Events.Base;
using UnityEngine;
using Hash128 = Unity.Entities.Hash128;

namespace TDGame.Events.Types
{
    
    [CreateAssetMenu(fileName = "GameEvent", menuName = "GameEvents/Hash128Event")]
    [Serializable]
    public class Hash128GameEvent : GameEvent<Hash128>
    {
    }
}