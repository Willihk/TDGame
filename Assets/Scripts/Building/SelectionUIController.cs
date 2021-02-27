using UnityEngine;

namespace TDGame.Building
{
    public class SelectionUIController : MonoBehaviour
    {
        [SerializeField]
        private GameObject selectionUI;

        public void DisplayUI(GameObject gameObject)
        {
            if (gameObject == null)
            {
                selectionUI.SetActive(false);
                return;
            }
            selectionUI.SetActive(true);
        }
    }
}
