using Mirror;
using TDGame.Network.Manager;

namespace TDGame.Network
{
    public class GameNetworkManager : Mirror_CustomNetworkManager
    {
        public static GameNetworkManager Instance;

        public override void Awake()
        {
            base.Awake();
            Instance = this;

            // spawnPrefabs.AddRange(networkedBuildingList.GetGameObjects());
            // spawnPrefabs.AddRange(networkedEnemyList.GetGameObjects());
        }
    }
}