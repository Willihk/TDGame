using System;
using Mirror;
using TDGame.Display.Data;
using UnityEngine;

namespace TDGame.Systems.Tower.Base
{
    [Obsolete]
    public class BaseNetworkedTower : NetworkBehaviour
    {
        public DisplayInfo DisplayInfo;
        
        [Header("Base")]
        public int price = 40;
    }
}