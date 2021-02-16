using UnityEngine;

namespace TDGame.Cursor
{
    [CreateAssetMenu(fileName = "Cursor", menuName = "CursorData", order = 0)]
    public class CursorData : ScriptableObject
    {
        public CursorState CursorState;
    }
}