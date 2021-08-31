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
            player = "random: " + Random.Range(0, int.MaxValue);
        }
    }
}