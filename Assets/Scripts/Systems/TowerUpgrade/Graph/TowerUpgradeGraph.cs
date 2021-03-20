using TDGame.Systems.TowerUpgrade.Graph.Nodes;
using UnityEngine;
using XNode;

namespace TDGame.Systems.TowerUpgrade.Graph
{
    [CreateAssetMenu(menuName = "Data/TowerUpgrade/UpgradeGraph")]
    public class TowerUpgradeGraph : NodeGraph
    {
        public TowerNode GetTower(GameObject tower)
        {
            return GetTower(tower.name);
        }
        
        public TowerNode GetTower(string towerName)
        {
            towerName = towerName.Replace("(Clone)", "");
            for (int i = 0; i < nodes.Count; i++)
            {
                if (!(nodes[i] is TowerNode graphNode)) 
                    continue;
                
                // TODO: Give each tower a unique ID to compare against
                if (graphNode.TowerPrefab.name == towerName)
                {
                    return graphNode;
                }
            }

            return null;
        }
    }
}