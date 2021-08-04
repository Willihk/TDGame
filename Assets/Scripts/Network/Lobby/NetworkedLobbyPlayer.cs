using System;
using UnityEngine;
using Mirror;
using TDGame.Events.Base;
using TDGame.Network.Messages.Player;
using TDGame.Network.Player;

namespace TDGame.Network.Lobby
{
    public class NetworkedLobbyPlayer : NetworkRoomPlayer
    {
        public static NetworkedLobbyPlayer LocalPlayer;
        
        [SyncVar]
        public int id;

        [SyncVar]
        public PlayerData playerData;
        
        [SerializeField]
        private GameEvent lobbyPlayersChangedEvent;

        public override void OnStartLocalPlayer()
        {
            LocalPlayer = this;
        }

        public override void OnStartClient()
        {
            lobbyPlayersChangedEvent.Raise();
        }
        
        public override void ReadyStateChanged(bool oldReadyState, bool newReadyState)
        {
            lobbyPlayersChangedEvent.Raise();
        }

        public override void IndexChanged(int oldIndex, int newIndex)
        {
            lobbyPlayersChangedEvent.Raise();
        }

        private void OnDestroy()
        {
            lobbyPlayersChangedEvent.Raise();
        }

        public void Setup(PlayerData playerData)
        {
            id = playerData.Id;
            this.playerData = playerData;
        }
    }
}