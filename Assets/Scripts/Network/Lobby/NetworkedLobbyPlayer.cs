using System;
using UnityEngine;
using Mirror;
using TDGame.Events.Base;

namespace TDGame.Network.Lobby
{
    public class NetworkedLobbyPlayer : NetworkRoomPlayer
    {
        [SerializeField]
        private GameEvent lobbyPlayersChangedEvent;

        public override void OnStartClient()
        {
            lobbyPlayersChangedEvent.Raise();
        }
        
        public override void OnClientEnterRoom()
        {
            Debug.Log("Client enter room");
        }

        public override void OnClientExitRoom()
        {
            Debug.Log("Client Exit room");
        }

        public override void ReadyStateChanged(bool oldReadyState, bool newReadyState)
        {
            Debug.Log("Client ready state changed " + newReadyState);
        }

        public override void IndexChanged(int oldIndex, int newIndex)
        {
            lobbyPlayersChangedEvent.Raise();
        }

        private void OnDestroy()
        {
            lobbyPlayersChangedEvent.Raise();
        }
    }
}