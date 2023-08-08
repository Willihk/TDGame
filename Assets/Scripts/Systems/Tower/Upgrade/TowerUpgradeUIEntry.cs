using TDGame.Events;
using TDGame.PrefabManagement;
using TDGame.Systems.Building;
using TDGame.Systems.Tower.Graph.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TDGame.Systems.Tower.Upgrade
{
    public class TowerUpgradeUIEntry : MonoBehaviour
    {
        [SerializeField]
        private Image towerImage;

        [SerializeField]
        private TextMeshProUGUI priceText;

        [SerializeField]
        private TextMeshProUGUI nameText;

        private int towerId;

        private Hash128 upgradeHash;

        public void Initialize(int id, TowerDetails upgradeDetails)
        {
            towerId = id;
            priceText.text = upgradeDetails.Price.ToString();
            nameText.text = upgradeDetails.Name;

            upgradeHash = PrefabManager.Instance.GetPrefabHash(upgradeDetails.name);
        }

        public void OnClickUpgrade()
        {
            EventManager.Instance.onClickTowerUpgrade.Raise(towerId, upgradeHash);
        }
    }
}