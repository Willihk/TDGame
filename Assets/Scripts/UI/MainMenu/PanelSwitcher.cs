using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDGame
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
