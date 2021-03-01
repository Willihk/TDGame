using Mirror;
using UnityEngine;

namespace TDGame.Systems.TowerUpgrade
{
    public class UpgradableTower : NetworkBehaviour
    {
        public GameObject upgradePrefab;

        public void UpgradeTower()
        {
            Cmd_UpgradeTower();
        }

        [Command]
        private void Cmd_UpgradeTower()
        {
            TowerUpgradeController.Instance.TryUpgradeTower(this);
        }
    }
}