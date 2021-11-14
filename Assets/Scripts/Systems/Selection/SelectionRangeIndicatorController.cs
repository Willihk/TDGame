using System;
using TDGame.Systems.TargetAcquisition.Implementations;
using UnityEngine;

namespace TDGame.Systems.Selection
{
    [Obsolete]
    public class SelectionRangeIndicatorController : MonoBehaviour
    {
        [SerializeField]
        private GameObject rangeIndicator;
        
        public void SetRangeIndicator(GameObject selectedGameObject)
        {
            if (selectedGameObject == null)
            {
                rangeIndicator.SetActive(false);
                return;
            }
            
            if (selectedGameObject.TryGetComponent(out RangeTargetAcquisition rangeComponent))
            {
                rangeIndicator.SetActive(true);

                float rangeDiameter = rangeComponent.rangeStat.stat.Value * 2;

                rangeIndicator.transform.position = selectedGameObject.transform.position + new Vector3(0, 0.2f, 0);
                rangeIndicator.transform.localScale = new Vector3(rangeDiameter, rangeDiameter, 0);
            }
            else
            {
                rangeIndicator.SetActive(false);
            }
        }
    }
}
