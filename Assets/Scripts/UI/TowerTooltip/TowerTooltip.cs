using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TDGame.UI.TowerTooltip
{
    public class TowerTooltip : MonoBehaviour
    {
        [SerializeField]
        public TextMeshProUGUI nameText;

        [SerializeField]
        public TextMeshProUGUI descriptionText;

        public void Initialize(string name, string desciption)
        {
            this.nameText.text = name;
            this.descriptionText.text = desciption;
        }
    }
}
