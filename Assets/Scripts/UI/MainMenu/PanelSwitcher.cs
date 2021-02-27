using UnityEngine;

namespace TDGame.UI.MainMenu
{
    public class PanelSwitcher : MonoBehaviour
    {

        [SerializeField]
        GameObject MainMenu;
        [SerializeField]
        GameObject ConnectionMenu;
        [SerializeField]
        GameObject OptionsMenu;

        public void SwitchActivePanel(string panelName)
        {
            MainMenu.SetActive(MainMenu.name.Equals(panelName));
            ConnectionMenu.SetActive(ConnectionMenu.name.Equals(panelName));
            OptionsMenu.SetActive(OptionsMenu.name.Equals(panelName));
        }
    }
}
