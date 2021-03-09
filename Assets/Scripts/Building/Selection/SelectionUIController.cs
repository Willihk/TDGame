using TDGame.Systems.Tower.Base;
using TDGame.Systems.TowerUpgrade;
using TDGame.UI.TowerUpgrade;
using UnityEngine;

namespace TDGame.Building.Selection
{
    public class SelectionUIController : MonoBehaviour
    {
        [SerializeField]
        private GameObject selectionUI;

        [SerializeField]
        private GameObject entryPrefab;

        [SerializeField]
        private Transform content;

        public void DisplayUI(GameObject gameObject)
        {
            if (gameObject == null)
            {
                selectionUI.SetActive(false);
                return;
            }

            selectionUI.SetActive(true);

            foreach (Transform child in content)
            {
                Destroy(child.gameObject);
            }

            foreach (var item in gameObject.GetComponents<UpgradableTower>())
            {
                var entryObject = Instantiate(entryPrefab, content);
                entryObject.GetComponent<TowerUpgradeEntry>().Initialize(item, item.upgradePrefab.name, item.upgradePrefab.TryGetComponent(out BaseNetworkedTower towerEntry) ? towerEntry.price - (gameObject.TryGetComponent(out BaseNetworkedTower tower) ? tower.price : 0) : 0);
            }
        }
    }
}
