using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TDGame.Systems.Enemy.Systems.Health
{
    public class HealthBarUIPool : MonoBehaviour
    {
        public static HealthBarUIPool Instance;

        [SerializeField]
        private GameObject healthBarPrefab;

        [SerializeField]
        private int initialPoolSize;

        [SerializeField]
        private Transform healthBarContainer;

        private Stack<Slider> healthBars;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            healthBars = new Stack<Slider>(initialPoolSize);
            for (var i = 0; i < initialPoolSize; i++)
            {
                SpawnSlider();
            }
        }

        private void SpawnSlider()
        {
            var newHealthBarObject = Instantiate(healthBarPrefab, Vector3.zero, Quaternion.identity, healthBarContainer);
            var newHealthBarSlider = newHealthBarObject.GetComponent<Slider>();
            healthBars.Push(newHealthBarSlider);
            newHealthBarSlider.gameObject.SetActive(false);
        }

        public Slider GetNextSlider()
        {
            if (healthBars.Count <= 0)
            {
                SpawnSlider();
            }

            var nextSlider = healthBars.Pop();
            nextSlider.gameObject.SetActive(true);
            return nextSlider;
        }

        public void ReturnSlider(Slider sliderToReturn)
        {
            sliderToReturn.gameObject.SetActive(false);
            healthBars.Push(sliderToReturn);
        }
    }
}