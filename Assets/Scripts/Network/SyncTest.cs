using Mirage;
using System.Collections;
using UnityEngine;

namespace TDGame.Assets.Scripts.Network
{
    public class SyncTest : NetworkBehaviour
    {
        [SyncVar]
        public string player;


        private void Awake()
        {
        }
    }
}