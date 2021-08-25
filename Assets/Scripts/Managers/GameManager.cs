using UnityEngine;

namespace TDGame.Managers
{
    public class GameManager : MonoBehaviour
    {

        public void LobbyStartGame()
        {
            Debug.Log("lobby start game");
            // Unload lobby scene
            // Load gameplay scene
            // Load map scene
            // Call setup event for gameplay
        }
    }
}