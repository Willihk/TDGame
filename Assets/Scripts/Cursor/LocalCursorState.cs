using System;
using UnityEngine;

namespace TDGame.Cursor
{
    [CreateAssetMenu(fileName = "Cursor", menuName = "Data/CursorData", order = 0)]
    public class LocalCursorState : ScriptableObject
    {
        [NonSerialized]
        public CursorState State;

        public void ResetState()
        {
            State = CursorState.None;
        }
    }
}