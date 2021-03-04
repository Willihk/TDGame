using TDGame.Systems.TowerUpgrade;
using UnityEngine;

namespace TDGame.Building
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
                entryObject.GetComponent<TowerUpgradeEntry>().Initialize(item, item.upgradePrefab.name);
            }
        }
    }
}
