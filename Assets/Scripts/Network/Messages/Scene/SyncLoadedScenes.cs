using System.Collections.Generic;

namespace TDGame.Network.Messages.Scene
{
    /// <summary>
    /// Sent from the server to clients.
    /// Contains information on which scenes are loaded on the server.
    /// The client will then make sure the given scenes are loaded.
    /// </summary>
    public struct SyncLoadedScenes
    {
        public List<string> LoadedSceneIDs;
    }
}