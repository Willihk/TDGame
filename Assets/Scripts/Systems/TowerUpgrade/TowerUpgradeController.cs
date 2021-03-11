using System;
using Mirror;
using TDGame.Building;
using TDGame.Building.Selection;
using TDGame.Network.Player;
using TDGame.Systems.Economy;
using TDGame.Systems.Grid.Data;
using TDGame.Systems.Grid.InGame;
using TDGame.Systems.Tower.Base;
using UnityEngine;

namespace TDGame.Systems.TowerUpgrade
{
    public class TowerUpgradeController : NetworkBehaviour
    {
        public static TowerUpgradeController Instance;

        [SerializeField]
        private InGamePlayerManager playerManager;

        [SerializeField]
        private BuildingList prefabList;

        private void Awake()
        {
            Instance = this;
        }

        [Server]
        public void TryUpgradeTower(UpgradableTower tower)
        {
            int newTowerPrice = tower.upgradePrefab.GetComponent<BaseNetworkedTower>().price;
            int oldTowerPrice = tower.gameObject.GetComponent<BaseNetworkedTower>().price;

            var playerEconomy = PlayerEconomyManager.Instance.GetEconomy(tower.connectionToClient);

            if (!playerEconomy.CanAfford(newTowerPrice - oldTowerPrice))
                return;

            PlayerEconomyManager.Instance.ReducesCurrencyForPlayer(
                playerManager.GetIdByConnection(tower.connectionToClient), newTowerPrice - oldTowerPrice);

            ReplaceTower(tower.upgradePrefab.name, tower.gameObject, tower.connectionToClient);
        }

        [Server]
        public void ReplaceTower(string prefabName, GameObject oldGameObject, NetworkConnection owner)
        {
            var prefab = prefabList.GetBuilding(prefabName);

            var spawned = Instantiate(prefab);
            spawned.transform.position = oldGameObject.transform.position;
            spawned.transform.rotation = oldGameObject.transform.rotation;

            // TODO: Rework to allow for easier access to grid area
            var area = oldGameObject.transform.Find("Model").gameObject.GetComponent<GridAreaController>().area;

            GridController.Instance.EmptyGridArea(GridType.Tower, area);
            NetworkServer.Spawn(spawned, owner);

            TargetUpdateSelection(owner, spawned);

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