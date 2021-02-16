using TDGame.Events.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TDGame.UI.BuildingList
{
    public class BuildingListEntry : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI nameText;

        [SerializeField]
        private Image image;

        [SerializeField]
        private GameEvent<string> OnClickBuyBuilding;

        private string prefabName;

        public void Initialize(string prefabName, string name)
        {
            this.prefabName = prefabName;
            nameText.text = name;
        }

        public void OnClick()
        {
            OnClickBuyBuilding.Raise(prefabName);
        }
    }
}