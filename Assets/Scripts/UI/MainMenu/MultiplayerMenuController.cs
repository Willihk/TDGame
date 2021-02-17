using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TDGame.Network;
using Mirror;

namespace TDGame
{
    public class MultiplayerMenuController : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void StartServer()
        {
            NetworkManager.singleton.StartServer();
        }
    }
}
