using TDGame.Systems.TowerTooltip;
using TDGame.Systems.TowerUpgrade;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TDGame.UI.TowerUpgrade
{
    public class TowerUpgradeEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private TextMeshProUGUI nameText;

        [SerializeField]
        private TextMeshProUGUI costText;

        [SerializeField]
        private Image image;

        private GameObject currentTower;
        private GameObject upgradeTower;
        public void Initialize(GameObject currentTower, GameObject upgradeTower, int cost)
        {
            this.currentTower = currentTower;
            this.upgradeTower = upgradeTower;
            nameText.text = upgradeTower.name;
            costText.text = cost == 0 ? "FREE" : cost.ToString();
        }

        public void OnClick()
        {
            TowerUpgradeController.Instance.CmdUpgradeTower(currentTower, upgradeTower.name);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            TowerTooltipController.Instance.DisplayUI(upgradeTower, this.GetComponent<RectTransform>());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TowerTooltipController.Instance.HideUI();
        }
    }
}
