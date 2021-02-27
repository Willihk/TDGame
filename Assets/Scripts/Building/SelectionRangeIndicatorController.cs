using TDGame.Systems.TargetAcquisition.Implementations;
using UnityEngine;

namespace TDGame
{
    public class SelectionRangeIndicatorController : MonoBehaviour
    {
        [SerializeField]
        private GameObject rangeIndicator;
        public void SetRangeIndicator(GameObject gameObject)
        {
            if (gameObject == null)
            {
                rangeIndicator.SetActive(false);
                return;
            }
            if (gameObject.TryGetComponent(out RangeTargetAcquisition rangeComponent))
            {
                rangeIndicator.SetActive(true);

                float rangeDiameter = rangeComponent.range * 2;

                rangeIndicator.transform.position = gameObject.transform.position + new Vector3(0, 0.2f, 0);
                rangeIndicator.transform.localScale = new Vector3(rangeDiameter, rangeDiameter, 0);
            }
            else
                rangeIndicator.SetActive(false);
        }
    }
}
