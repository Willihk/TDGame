using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TDGame
{
    public class EconomyController : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI EconomyText;

        public void UpdateEconomy(int num)
        {
            EconomyText.text = num.ToString();
        }
    }
}
