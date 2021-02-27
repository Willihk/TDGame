using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

/*
	Documentation: https://mirror-networking.com/docs/Guides/NetworkBehaviour.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

namespace TDGame.Building
{
    public class BuildController : NetworkBehaviour
    {
        public static BuildController Instance;

        [SerializeField]
        List<GameObject> prefabs = new List<GameObject>();

        void LoadPrefabs()
        {
            prefabs = NetworkManager.singleton.spawnPrefabs;
        }

        public GameObject GetBuilding(string name)
        {
            return prefabs.First(x => x.name == name);
        }

        public GameObject GetBuilding(int index)
        {
            return prefabs[index];
        }

        public int GetIndexOfBuildingName(string name)
        {
            return prefabs.Select(x => x.name).ToList().IndexOf(name);
        }


        /// <summary>
        /// This is invoked for NetworkBehaviour objects when they become active on the server.
        /// <para>This could be triggered by NetworkServer.Listen() for objects in the scene, or by NetworkServer.Spawn() for objects that are dynamically created.</para>
        /// <para>This will be called for objects on a "host" as well as for object on a dedicated server.</para>
        /// </summary>
        public override void OnStartServer()
        {
            Instance = this;

            LoadPrefabs();
        }

        /// <summary>
        /// Called on every NetworkBehaviour when it is activated on a client.
        /// <para>Objects on the host have this function called, as there is a local client on the host. The values of SyncVars on object are guaranteed to be initialized correctly with the latest state from the server when this function is called on the client.</para>
        /// </summary>
        public override void OnStartClient()
        {
            Instance = this;
            LoadPrefabs();
        }
    }
}