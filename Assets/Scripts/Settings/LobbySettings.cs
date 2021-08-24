using TDGame.Map.Data;
using UnityEngine;

namespace TDGame.Settings
{
    [CreateAssetMenu(fileName = "LobbySettings", menuName = "Data/Settings/LobbySettings", order = 0)]
    public class LobbySettings : ScriptableObject
    {
        public MapDetails selectedMap;
    }
}
