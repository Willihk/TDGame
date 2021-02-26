using TMPro;
using UnityEngine;

namespace TDGame.UI.Lobby
{
    public class LobbyPlayerListEntry : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI playerNameText;

        public void Initialize(string name)
        {
            playerNameText.text = name;
        }
    }
}