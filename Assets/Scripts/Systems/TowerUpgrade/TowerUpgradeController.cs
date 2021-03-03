using System;
using Mirror;
using TDGame.Building;
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
            
            NetworkServer.Spawn(spawned, owner);

            NetworkServer.Destroy(oldGameObject);
        }
    }
}