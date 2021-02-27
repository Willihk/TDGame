using TMPro;
using UnityEngine;

namespace TDGame.UI.Lobby
{
    public class LobbyPlayerListEntry : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI playerNameText;

        [SerializeField]
        private GameObject readyIndicator;

        public void Initialize(string name, bool isReady)
        {
            playerNameText.text = name;
            readyIndicator.gameObject.SetActive(isReady);
        }
    }
}