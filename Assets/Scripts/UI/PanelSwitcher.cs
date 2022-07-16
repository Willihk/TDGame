using UnityEngine;

namespace TDGame.UI
{
    public class PanelSwitcher : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] panels;

        public void SwitchPanel(string panelName)
        {
            foreach (var panel in panels)
            {
                panel.SetActive(panel.name == panelName);
            }
        }
        public void SwitchPanel(GameObject target)
        {
            SwitchPanel(target.name);
        }
    }
}