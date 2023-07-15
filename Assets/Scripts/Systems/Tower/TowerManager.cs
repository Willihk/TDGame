using System;
using System.IO;
using MessagePack;
using TDGame.Events;
using TDGame.Managers;
using TDGame.Network.Components;
using TDGame.Network.Components.Messaging;
using TDGame.PrefabManagement;
using TDGame.Systems.Building;
using TDGame.Systems.Economy;
using TDGame.Systems.Tower.Upgrade.Messages;
using TDGame.Systems.Tower.Upgrade.Messages.Client;
using TDGame.Systems.Tower.Upgrade.Messages.Server;
using UnityEngine;

namespace TDGame.Systems.Tower
{
    public class TowerManager : MonoBehaviour
    {
        private BaseMessagingManager messagingManager;

        private TransportClientWrapper clientWrapper;
        private TransportServerWrapper serverWrapper;
        
        private void Start()
        {
            clientWrapper = CustomNetworkManager.Instance.clientWrapper;
            serverWrapper = CustomNetworkManager.Instance.serverWrapper;
            messagingManager = BaseMessagingManager.Instance;
            
            messagingManager.RegisterNamedMessageHandler<TowerUpgradeMessage>(HandleTowerUpgradeMessage);
            messagingManager.RegisterNamedMessageHandler<RequestTowerUpgrade>(HandleRequestTowerUpgrade);

            EventManager.Instance.onClickTowerUpgrade.EventListeners += OnClickTowerUpgrade;
        }

        private void OnClickTowerUpgrade(int id, Hash128 upgrade)
        {
            messagingManager.SendNamedMessageToServer(new RequestTowerUpgrade() {TowerId = id, UpgradeHash = upgrade});
        }


        void HandleRequestTowerUpgrade(TDNetworkConnection sender, Stream stream)
        {
            var message = MessagePackSerializer.Deserialize<RequestTowerUpgrade>(stream); 
            
            //TODO: Validation

            // if (!BuildingManager.Instance.GetHashById(message.TowerId, out var builtHash))
            //     return;

            var upgradeDetails = PrefabManager.Instance.GetTowerDetails(message.UpgradeHash);

            int playerId = NetworkPlayerManager.Instance.GetPlayerId(sender);
            var economy = PlayerEconomyManager.Instance.GetEconomy(playerId);

            if (!economy.CanAfford(upgradeDetails.Price))
                return;

            economy.Purchase(upgradeDetails.Price);
            PlayerEconomyManager.Instance.SyncEconomies();


            BuildingManager.Instance.Server_RemoveBuilding(message.TowerId);
            
            messagingManager.SendNamedMessageToAll(new TowerUpgradeMessage() {TowerId = message.TowerId, UpgradeHash = message.UpgradeHash});
        }
        
        void HandleTowerUpgradeMessage(TDNetworkConnection sender, Stream stream)
        {
            
        }
    }
}