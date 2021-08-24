namespace TDGame.Network.Messages.Scene
{
    /// <summary>
    /// Sent from server
    /// 
    /// Reciever should unload the scene in this message.
    /// </summary>
    public struct UnloadScene
    {
        public string SceneID;
    }
}
