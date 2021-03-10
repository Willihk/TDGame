using System;
using Mirror;
using TDGame.Building;
using TDGame.Building.Selection;
using TDGame.Systems.Grid.Data;
using TDGame.Systems.Grid.InGame;
using UnityEngine;

namespace TDGame.Systems.TowerUpgrade
{
    public class TowerUpgradeController : NetworkBehaviour
    {
        public static TowerUpgradeController Instance;

        [SerializeField]
        private BuildingList prefabList;

        private void Awake()
        {
            Instance = this;
        }

        [Server]
        public void TryUpgradeTower(UpgradableTower tower)
        {
            ReplaceTower(tower.upgradePrefab.name, tower.gameObject, tower.connectionToClient);
        }

        [Server]
        public void ReplaceTower(string prefabName, GameObject oldGameObject, NetworkConnection owner)
        {
            var prefab = prefabList.GetBuilding(prefabName);

            var spawned = Instantiate(prefab);
            spawned.transform.position = oldGameObject.transform.position;
            spawned.transform.rotation = oldGameObject.transform.rotation;

            TargetUpdateSelection(owner, spawned);

            // TODO: Rework to allow for easier access to grid area
            var area = oldGameObject.transform.Find("Model").gameObject.GetComponent<GridAreaController>().area;

            GridController.Instance.EmptyGridArea(GridType.Tower, area);
            NetworkServer.Spawn(spawned, owner);

            if (spawned.transform.Find("Model").gameObject.TryGetComponent(out GridAreaController areaController))
            {
                GridController.Instance.PlaceTowerOnGrid(spawned, areaController.CalculateArea());
            }

            NetworkServer.Destroy(oldGameObject);
        }

        [TargetRpc]
        private void TargetUpdateSelection(NetworkConnection target, GameObject tower)
        {
            SelectionController.Instance.ChangeSelection(tower);
        }
    }
}