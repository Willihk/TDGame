using System;
using TDGame.Events;
using TDGame.Player;
using TDGame.Systems.Economy;
using TMPro;
using UnityEngine;

namespace TDGame.UI.InGame
{
    public class CurrencyUIController : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI text;

        [SerializeField]
        private LocalPlayer localPlayer;

        private void Start()
        {
            EventManager.Instance.onEconomyChanged.EventListeners += UpdateText;
        }
        
        private void OnDestroy()
        {
            EventManager.Instance.onEconomyChanged.EventListeners -= UpdateText;
        }

        private void UpdateText(int playerId)
        {
            Debug.Log("other " + playerId + " local: " + localPlayer.playerId);
            if (localPlayer.playerId == playerId)
            {
                Debug.Log("New currency: " + PlayerEconomyManager.Instance.GetEconomy(playerId).currency);
                text.text= PlayerEconomyManager.Instance.GetEconomy(playerId).currency.ToString();
            }
        }
    }
}