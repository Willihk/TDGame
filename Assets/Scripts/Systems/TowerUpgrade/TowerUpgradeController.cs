using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using TDGame.Building;
using TDGame.Building.Selection;
using TDGame.Network.Player;
using TDGame.Systems.Economy;
using TDGame.Systems.Grid.Data;
using TDGame.Systems.Grid.InGame;
using TDGame.Systems.Tower.Base;
using TDGame.Systems.Tower.Graph;
using TDGame.Systems.Tower.Graph.Nodes;
using UnityEngine;

namespace TDGame.Systems.TowerUpgrade
{
    public class TowerUpgradeController : NetworkBehaviour
    {
        public static TowerUpgradeController Instance;

        [SerializeField]
        private TowerGraph towerGraph;

        [SerializeField]
        private InGamePlayerManager playerManager;

        [SerializeField]
        private BuildingList prefabList;

        private void Awake()
        {
            Instance = this;
        }

        public List<GameObject> GetUpgradesForTower(GameObject tower)
        {
            TowerNode towerNode = towerGraph.GetTower(tower);

            if (!towerNode)
            {
                return new List<GameObject>();
            }

            var upgrades = new List<GameObject>();
            
            foreach (var connection in towerNode.GetOutputPort("Next").GetConnections())
            {
                if (connection.node is TowerNode upgradeNode)
                {
                    upgrades.Add(upgradeNode.TowerPrefab);
                }
            }

            return upgrades;
        }

        [Command(ignoreAuthority = true)]
        public void CmdUpgradeTower(GameObject tower, string upgradeName, NetworkConnectionToClient sender = null)
        {
            
            var towerOwner = tower.GetComponent<NetworkIdentity>().connectionToClient;
            if (sender != towerOwner) // Makes sure only the tower owner can upgrade
            {
                return;
            }

            
            TowerNode towerNode = towerGraph.GetTower(upgradeName);
            if (!towerNode)
                return;
            
            TryUpgradeTower(tower, towerNode.TowerPrefab);
        }

        [Server]
        public void TryUpgradeTower(GameObject tower, GameObject upgradePrefab)
        {
            var towerOwner = tower.GetComponent<NetworkIdentity>().connectionToClient;
            
            int newTowerPrice = upgradePrefab.GetComponent<BaseNetworkedTower>().price;
            int oldTowerPrice = tower.gameObject.GetComponent<BaseNetworkedTower>().price;

            var playerEconomy = PlayerEconomyManager.Instance.GetEconomy(towerOwner);

            if (!playerEconomy.CanAfford(newTowerPrice - oldTowerPrice))
                return;

            PlayerEconomyManager.Instance.ReducesCurrencyForPlayer(
                playerManager.GetIdByConnection(towerOwner), newTowerPrice - oldTowerPrice);

            ReplaceTower(upgradePrefab.name, tower.gameObject, towerOwner);
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