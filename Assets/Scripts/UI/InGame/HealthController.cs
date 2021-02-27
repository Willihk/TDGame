using TMPro;
using UnityEngine;

namespace TDGame.UI.InGame
{
    public class HealthController : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI HealthText;



        public void UpdateHealth(float f)
        {
            HealthText.text = f.ToString();
        }
    }
}
