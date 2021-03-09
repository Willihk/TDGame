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
        private TextMeshProUGUI costText;

        [SerializeField]
        private Image image;

        private UpgradableTower component;

        public void Initialize(UpgradableTower component, string name, int cost)
        {
            this.component = component;
            nameText.text = name;
            costText.text = cost.ToString();
        }

        public void OnClick()
        {
            component.UpgradeTower();
        }
    }
}
