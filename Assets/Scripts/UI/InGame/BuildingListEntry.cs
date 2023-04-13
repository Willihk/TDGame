using TDGame.Cursor;
using TDGame.Events.Base;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TDGame.UI.InGame
{
    public class BuildingListEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private TextMeshProUGUI nameText;

        [SerializeField]
        private TextMeshProUGUI costText;

        [SerializeField]
        private Image image;

        [SerializeField]
        private GameEvent<string> OnClickBuyBuilding;

        [SerializeField]
        private LocalCursorState localCursorState;

        private string prefabName;

        public void Initialize(string prefabName, string name, int cost)
        {
            this.prefabName = prefabName;
            nameText.text = name;
            costText.text = cost == 0 ? "FREE" : cost.ToString();
        }

        public void OnClick()
        {
            if (localCursorState.State == CursorState.None)
            {
                localCursorState.State = CursorState.Placing;
                // OnClickBuyBuilding.Raise(prefabName);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // TowerTooltipController.Instance.DisplayUI(prefabName, this.GetComponent<RectTransform>());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // TowerTooltipController.Instance.HideUI();
        }
    }
}