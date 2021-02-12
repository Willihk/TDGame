using UnityEngine;
using Mirror;
using TDGame.Network.Message;
using System.Linq;
using TDGame.Network.Player;

/*
	Documentation: https://mirror-networking.com/docs/Components/NetworkManager.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkManager.html
*/

namespace TDGame.Network
{
    public class TDGameNetworkManager : NetworkManager
    {

        public override void OnStartServer()
        {
            base.OnStartServer();

            NetworkServer.RegisterHandler<CreatePlayerMessage>(OnCreatePlayer);
        }

        /// <summary>
        /// Called on the client when connected to a server.
        /// <para>The default implementation of this function sets the client as ready and adds a player. Override the function to dictate what happens when the client connects.</para>
        /// </summary>
        /// <param name="conn">Connection to the server.</param>
        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);

            CreatePlayerMessage message = new CreatePlayerMessage { Name = "Player " + Random.Range(0, 10000) };

            conn.Send<CreatePlayerMessage>(message);
        }

        void OnCreatePlayer(NetworkConnection conn, CreatePlayerMessage message)
        {
            PlayerManager.Instance.PlayerConnected(new PlayerData { ConnectionId = conn.connectionId, Name = message.Name });

            GameObject gameobject = Instantiate(playerPrefab);
            NetworkServer.AddPlayerForConnection(conn, gameobject);
        }
    }
}