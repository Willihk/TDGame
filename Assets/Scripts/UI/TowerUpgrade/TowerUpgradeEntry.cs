using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TDGame.Systems.TowerUpgrade;
using TDGame.Events.Base;

namespace TDGame
{
    public class TowerUpgradeEntry : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI nameText;

        [SerializeField]
        private Image image;

        private UpgradableTower component;

        public void Initialize(UpgradableTower component, string name)
        {
            this.component = component;
            nameText.text = name;
        }

        public void OnClick()
        {
            component.UpgradeTower();
        }
    }
}
