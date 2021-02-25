using Mirror;
using UnityEngine;

namespace TDGame.Systems.Turrets.Base
{
    public class BaseNetworkedTurret : NetworkBehaviour
    {
        [Header("Base")]
        public int price = 40;
    }
}