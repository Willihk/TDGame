﻿using MessagePack;

namespace TDGame.Network.Messages.Scene
{
    /// <summary>
    /// Sent from server
    /// 
    /// Reciever should load the scene in this message.
    /// </summary>
    [MessagePackObject]
    public struct LoadSceneMessage
    {
        [Key(0)]
        public string SceneID;
    }
}