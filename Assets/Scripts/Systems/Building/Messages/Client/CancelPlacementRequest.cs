using MessagePack;

namespace TDGame.Systems.Building.Messages.Client
{
    /// <summary>
    /// Sent from client to server
    /// </summary>
    [MessagePackObject]
    public struct CancelPlacementRequest
    {
    }
}