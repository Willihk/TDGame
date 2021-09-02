﻿using MessagePack;

namespace TDGame.Network.Messages.Scene
{
    /// <summary>
    /// Sent from server
    /// 
    /// Reciever should unload the scene in this message.
    /// </summary>
    [MessagePackObject]
    public struct UnloadScene
    {
        [Key(0)]
        public string SceneID;
    }
}
