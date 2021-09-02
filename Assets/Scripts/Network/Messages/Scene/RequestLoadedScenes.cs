using MessagePack;

namespace TDGame.Network.Messages.Scene
{
    /// <summary>
    /// Sent from client to the server.
    /// Indicates the client wants to get the loaded scenes.
    ///
    /// Server should send a separate "LoadedScenes" message
    /// </summary>
    [MessagePackObject]
    public struct RequestLoadedScenes { }
}