using Mirror;
using Sirenix.Serialization;
using UnityEngine;

namespace TDGame.Systems.Tower.Modules.Hit.Base
{
    public abstract class TowerFiringController : NetworkBehaviour
    {
        public abstract void Fire();
    }
}