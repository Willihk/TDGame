using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TDGame.Display.Data
{
    [Serializable]
    public struct DisplayInfo
    {
        public string Name;
        [TextArea]
        public string Description;
        
        [AssetSelector]
        public Sprite Icon;
    }
}