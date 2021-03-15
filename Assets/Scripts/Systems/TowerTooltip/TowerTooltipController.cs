using System.Text;
using TDGame.Building;
using TDGame.Systems.Stats;
using TDGame.Systems.Tower.Base;
using UnityEngine;

namespace TDGame.Systems.TowerTooltip
{
    public class TowerTooltipController : MonoBehaviour
    {
        public static TowerTooltipController Instance;

        [SerializeField]
        private GameObject tooltip;

        [SerializeField]
        private Transform content;

        [SerializeField]
        private BuildingList buildingList;

        private void Awake()
        {
            Instance = this;
            tooltip.SetActive(false);
        }

        public void DisplayUI(string prefabName, RectTransform thisthing)
        {
            DisplayUI(buildingList.GetBuilding(prefabName), thisthing);
        }

        public void DisplayUI(GameObject towerInfo, RectTransform thisthing)
        {
            if (towerInfo == null)
            {
                tooltip.SetActive(false);
                return;
            }


            if (tooltip.TryGetComponent(out UI.TowerTooltip.TowerTooltip towerTooltip))
            {
                StringBuilder sb = new StringBuilder();

                foreach (var item in towerInfo.GetComponentsInChildren<StatWrapper>())
                {
                    sb.Append($"\n<color=#FFD800>{item.stat.Name}:\n {item.stat.BaseValue}</color>");
                }

                if (towerInfo.TryGetComponent(out BaseNetworkedTower tower))
                {
                    sb.Insert(0, $"{tower.DisplayInfo.Description}\n");
                    towerTooltip.Initialize($"<color=#9BE6FF>{tower.DisplayInfo.Name}</color>", sb.ToString());
                }
                else
                    towerTooltip.Initialize("noname", "");

                float textSpace = 0;
                if (towerTooltip.descriptionText.text != "")
                    textSpace = 10;

                towerTooltip.descriptionText.GetComponent<RectTransform>().offsetMax = new Vector2(0, - (towerTooltip.nameText.preferredHeight + textSpace + 5));

                tooltip.GetComponent<RectTransform>().sizeDelta = new Vector2(400, towerTooltip.descriptionText.preferredHeight + towerTooltip.nameText.preferredHeight + textSpace + 10);
            }

            tooltip.transform.position = thisthing.position;
            tooltip.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
            tooltip.transform.position += new Vector3(0, thisthing.sizeDelta.y, 0);

            tooltip.SetActive(true);
        }

        public void HideUI()
        {
            tooltip.SetActive(false);
        }
    }
}
