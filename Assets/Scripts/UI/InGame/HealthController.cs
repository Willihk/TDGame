using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TDGame
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
