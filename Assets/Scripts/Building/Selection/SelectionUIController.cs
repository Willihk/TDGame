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
        private Transform parent;

        public void DisplayUI(GameObject gameObject)
        {
            if (gameObject == null)
            {
                selectionUI.SetActive(false);
                foreach (var child in parent)
                {
                    Destroy(parent.transform.GetChild(0).gameObject);
                }
                return;
            }

            selectionUI.SetActive(true);

            foreach (var item in gameObject.GetComponents<UpgradableTower>())
            {
                var entryObject = Instantiate(entryPrefab, parent);
                entryObject.GetComponent<TowerUpgradeEntry>().Initialize(item, item.name);
            }
        }
    }
}
