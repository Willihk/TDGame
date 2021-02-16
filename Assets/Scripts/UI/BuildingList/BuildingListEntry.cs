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

        public void Initialize(string name)
        {
            nameText.text = name;
        }
    }
}