using Doozy.Engine;
using TDGame.Systems.Tower.Base;
using TDGame.Systems.Tower.Upgrade;
using Unity.Mathematics;
using UnityEngine;

namespace TDGame.UI.TowerUpgrade
{
    public class TowerUISelectionController : MonoBehaviour
    {
        [SerializeField]
        private GameObject selectionUI;

        [SerializeField]
        private GameObject entryPrefab;

        [SerializeField]
        private Transform content;

        public void DisplayUI(GameObject selectedObject)
        {
            if (selectedObject == null || !selectedObject.TryGetComponent(out BaseNetworkedTower selectedTower))
            {
                GameEventMessage.SendEvent("Close TowerUI");
                return;
            }

            GameEventMessage.SendEvent("Open TowerUI");

            foreach (Transform child in content)
            {
                Destroy(child.gameObject);
            }

            var upgrades = TowerUpgradeController.Instance.GetUpgradesForTower(selectedObject);

            foreach (var upgrade in upgrades)
            {
                var entryObject = Instantiate(entryPrefab, content);
                var entry = entryObject.GetComponent<TowerUpgradeEntry>();

                int currentTowerCost = selectedTower.price;
                int upgradeTowerCost = upgrade.GetComponent<BaseNetworkedTower>().price;

                entry.Initialize(selectedObject, upgrade, math.max(upgradeTowerCost - currentTowerCost, 0));
            }
        }
    }
}