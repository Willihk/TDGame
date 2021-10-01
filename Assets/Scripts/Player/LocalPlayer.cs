using UnityEngine;

namespace TDGame.Player
{
    [CreateAssetMenu(fileName = "LocalPlayer", menuName = "Data/Player/LocalPlayer", order = 0)]
    public class LocalPlayer : ScriptableObject
    {
        public int playerId;

        public void SetupLocalPlayer(int assignedId)
        {
            playerId = assignedId;
        }
    }
}