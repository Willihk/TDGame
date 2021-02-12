using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TDGame.UI.PlayerList
{
    public class PlayerListEntry : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI playerNameText;
        [SerializeField]
        TextMeshProUGUI playerMoneyText;

        public void Initialize(string name, int money = 100)
        {
            playerNameText.text = name;
            playerMoneyText.text = money.ToString();
        }
    }
}
