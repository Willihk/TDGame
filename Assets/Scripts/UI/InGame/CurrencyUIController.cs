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
            if (localPlayer.playerId == playerId)
            {
                text.text= PlayerEconomyManager.Instance.GetEconomy(playerId).currency.ToString();
            }
        }
    }
}