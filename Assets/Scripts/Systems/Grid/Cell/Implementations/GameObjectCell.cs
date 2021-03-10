using System;
using TDGame.Systems.Grid.Cell.Base;
using TDGame.Systems.Grid.Cell.Interfaces;
using UnityEngine;

namespace TDGame.Systems.Grid.Cell.Implementations
{
    [Serializable]
    public class GameObjectCell : BaseCell, IDisplayCell
    {
        public GameObject Owner;

        public override string ToString()
        {
            return Owner.ToString();
        }
    }
}