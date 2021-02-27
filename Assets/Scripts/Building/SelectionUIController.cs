using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDGame
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
