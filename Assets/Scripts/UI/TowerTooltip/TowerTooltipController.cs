using System.Text;
using TDGame.PrefabManagement;
using TDGame.Systems.Tower.Attack.Implementations.Projectile.Components;
using TDGame.Systems.Tower.Attack.Windup.Components;
using TDGame.Systems.Tower.Graph.Data;
using TDGame.Systems.Tower.Targeting.Components;
using UnityEngine;
using Hash128 = Unity.Entities.Hash128;

namespace TDGame.UI.TowerTooltip
{
     public class TowerTooltipController : MonoBehaviour
    {
        public static TowerTooltipController Instance;

        [SerializeField]
        private GameObject tooltip;

        private void Awake()
        {
            Instance = this;
            HideUI();
        }

        public void DisplayUI(string prefabName, RectTransform hoveredTransform)
        {
            DisplayUI(PrefabManager.Instance.GetPrefabHash(prefabName), hoveredTransform);
        }

        public void DisplayUI(Hash128 hash, RectTransform hoveredTransform)
        {
            if (hash == default)
            {
                HideUI();
                return;
            }

            var details = PrefabManager.Instance.GetTowerDetails(hash);

            if (tooltip.TryGetComponent(out TowerTooltip towerTooltip))
            {
                StringBuilder sb = new ();

                AddStats(sb, details);

                if (details != null)
                {
                    sb.Insert(0, $"{details.Description}\n");
                    towerTooltip.Initialize($"<color=#9BE6FF>{details.Name}</color>", sb.ToString());
                }
                else
                    towerTooltip.Initialize("noname", "");

                float textSpace = 0;
                if (towerTooltip.descriptionText.text != "")
                    textSpace = 10;

                towerTooltip.descriptionText.GetComponent<RectTransform>().offsetMax = new Vector2(0, - (towerTooltip.nameText.preferredHeight + textSpace + 5));

                tooltip.GetComponent<RectTransform>().sizeDelta = new Vector2(400, towerTooltip.descriptionText.preferredHeight + towerTooltip.nameText.preferredHeight + textSpace + 10);
            }

            var position = hoveredTransform.position;
            tooltip.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
            position += new Vector3(0, hoveredTransform.sizeDelta.y, 0);
            
            tooltip.transform.position = position;

            tooltip.SetActive(true);
        }

        public void HideUI()
        {
            tooltip.SetActive(false);
        }

        private static void AddStats(StringBuilder sb, TowerDetails details)
        {
            // foreach (var item in towerInfo.GetComponentsInChildren<StatWrapper>())
            // {
            //     sb.Append($"\n<color=#FFD800>{item.stat.Name}:\n {item.stat.BaseValue}</color>");
            // }
            if (details.TowerReference.TryGetComponent(out TargetRangeAuthoring rangeAuthoring))
            {
                sb.Append($"\n<color=#FFD800>Range:\n {rangeAuthoring.Range}</color>");
            }
            if (details.TowerReference.TryGetComponent(out BasicWindupAuthoring windupAuthoring))
            {
                sb.Append($"\n<color=#FFD800>Fire Rate:\n {1/windupAuthoring.WindupTime}</color>");
            }  
            if (details.TowerReference.TryGetComponent(out ProjectilePrefabAuthoring prefabAuthoring))
            {
                if (prefabAuthoring.Prefab.TryGetComponent(out ProjectileDamageAuthoring damageAuthoring))
                {
                    sb.Append($"\n<color=#FFD800>Damage:\n {damageAuthoring.Value}</color>");
                } 
                if (prefabAuthoring.Prefab.TryGetComponent(out ProjectileMovementSpeedAuthoring movementSpeed))
                {
                    sb.Append($"\n<color=#FFD800>Projectile Speed:\n {1/movementSpeed.Value}</color>");
                }
            }
        }
    }
}