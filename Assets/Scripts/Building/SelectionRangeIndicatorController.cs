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
            if (gameObject != null)
            {
                if (gameObject.GetComponent<RangeTargetAcquisition>())
                {
                    rangeIndicator.SetActive(true);
                    float range = gameObject.GetComponent<RangeTargetAcquisition>().range;
                    rangeIndicator.transform.position = gameObject.transform.position + new Vector3(0, 0.2f, 0);
                    rangeIndicator.transform.localScale = new Vector3(range * 2, range * 2, 0);
                }
                else
                    rangeIndicator.SetActive(false);
            }
            else
                rangeIndicator.SetActive(false);
        }
    }
}
