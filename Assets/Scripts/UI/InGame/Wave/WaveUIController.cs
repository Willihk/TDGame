using System.Globalization;
using TMPro;
using UnityEngine;

namespace TDGame.UI.InGame.Wave
{
    public class WaveUIController : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI waveText;

        public void UpdateWave(int wave)
        {
            waveText.text = wave.ToString(CultureInfo.InvariantCulture);
        }
    }
}