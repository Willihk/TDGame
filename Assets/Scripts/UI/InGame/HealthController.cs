using TMPro;
using UnityEngine;

namespace TDGame.UI.InGame
{
    public class HealthController : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI HealthText;



        public void UpdateHealth(int value)
        {
            HealthText.text = value.ToString();
        }
    }
}
