using TMPro;
using UnityEngine;

namespace TDGame.UI.InGame
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
