
using UnityEngine;

namespace TDGame.Utility.GameObjects
{
    public class GameObjectToggle : MonoBehaviour
    {
        public void ToggleObject(GameObject obj)
        {
            obj.SetActive(!obj.activeSelf);
        }
    }
}
