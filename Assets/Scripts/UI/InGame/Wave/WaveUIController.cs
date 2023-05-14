using System;
using System.Globalization;
using TDGame.Events;
using TDGame.Network.Components;
using TMPro;
using UnityEngine;

namespace TDGame.UI.InGame.Wave
{
    public class WaveUIController : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI waveText;

        [SerializeField]
        private Transform nextWaveButton;

        private void Start()
        {
            EventManager.Instance.onWaveCompleted.EventListeners += OnWaveCompleted;
            EventManager.Instance.onWaveStarted.EventListeners += OnWaveStart;

            nextWaveButton.gameObject.SetActive(CustomNetworkManager.Instance.serverWrapper.isListening);
        }

        private void OnDestroy()
        {
            EventManager.Instance.onWaveCompleted.EventListeners -= OnWaveCompleted;
            EventManager.Instance.onWaveStarted.EventListeners -= OnWaveStart;
        }

        private void OnWaveCompleted(int wave)
        {
            if (CustomNetworkManager.Instance.serverWrapper.isListening)
            {
                nextWaveButton.gameObject.SetActive(true);
            }
        }

        private void OnWaveStart(int wave)
        {
            nextWaveButton.gameObject.SetActive(false);
            waveText.text = wave.ToString(CultureInfo.InvariantCulture);
        }
    }
}