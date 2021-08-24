namespace TDGame.Network.Messages.Scene
{
    /// <summary>
    /// Sent from server
    /// 
    /// Reciever should load the scene in this message.
    /// </summary>
    public struct LoadScene
    {
        public string SceneID;
    }
}