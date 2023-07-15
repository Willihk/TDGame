using System;
using System.Collections.Generic;
using Doozy.Engine.UI;
using TDGame.Events;
using TDGame.PrefabManagement;
using TDGame.Systems.Building;
using TDGame.Systems.Grid.InGame;
using TDGame.Systems.Tower.Graph.Data;
using UnityEngine;

namespace TDGame.Systems.Tower.Upgrade
{
    public class TowerUpgradeUIController : MonoBehaviour
    {
        [SerializeField]
        private UIView view;
        
        [SerializeField]
        private Transform content;

        [SerializeField]
        private GameObject entryPrefab;


        private void OnEnable()
        {
            EventManager.Instance.onClickTower.EventListeners += OnTowerClicked;
        }

        private void OnDisable()
        {
            EventManager.Instance.onClickTower.EventListeners -= OnTowerClicked;
        }

        void OnTowerClicked(int id)
        {
            if (BuildingManager.Instance.GetDetailsOfTower(id, out var details))
            {
                var upgrades = PrefabManager.Instance.GetTowerUpgrades(details);
                PopulateView(upgrades, id);
            }
            else
            {
                view.Hide();
            }
        }

        void PopulateView(TowerDetails[] towers, int id)
        {
            foreach (Transform child in content)
            {
                Destroy(child.gameObject);
            }

            foreach (var item in towers)
            {
                var newEntry = Instantiate(entryPrefab, content);
                newEntry.GetComponent<TowerUpgradeUIEntry>().Initialize(id, item);
            }

            view.Show();
        }
    }
}