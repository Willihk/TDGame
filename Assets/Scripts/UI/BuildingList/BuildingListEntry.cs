using TDGame.Cursor;
using TDGame.Events.Base;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TDGame.UI.BuildingList
{
    public class BuildingListEntry : MonoBehaviour
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
            costText.text = cost.ToString();
        }

        public void OnClick()
        {
            if (localCursorState.State == CursorState.None)
            {
                localCursorState.State = CursorState.Placing;
                OnClickBuyBuilding.Raise(prefabName);
            }
        }
    }
}