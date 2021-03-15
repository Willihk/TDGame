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

        private UpgradableTower component;

        public void Initialize(UpgradableTower component, string name, int cost)
        {
            this.component = component;
            nameText.text = name;
            costText.text = cost == 0 ? "FREE" : cost.ToString();
        }

        public void OnClick()
        {
            component.UpgradeTower();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            TowerTooltipController.Instance.DisplayUI(component.upgradePrefab, this.GetComponent<RectTransform>());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TowerTooltipController.Instance.HideUI();
        }
    }
}
