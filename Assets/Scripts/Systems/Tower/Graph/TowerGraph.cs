using TDGame.Systems.Tower.Graph.Nodes;
using UnityEngine;
using XNode;

namespace TDGame.Systems.Tower.Graph
{
    [CreateAssetMenu(menuName = "Data/TowerUpgrade/UpgradeGraph")]
    public class TowerGraph : NodeGraph
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