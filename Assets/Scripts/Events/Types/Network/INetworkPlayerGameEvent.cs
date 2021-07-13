using System;
using Mirage;
using TDGame.Events.Base;
using UnityEngine;

namespace TDGame.Events.Types.Network
{
    [CreateAssetMenu(fileName = "GameEvent", menuName = "GameEvents/INetworkPlayerEvent")]
    [Serializable]
    public class INetworkPlayerGameEvent : GameEvent<INetworkPlayer>
    {
    }
}