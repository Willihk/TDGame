using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TDGame.Systems.Tower.Graph.Data;
using TDGame.Systems.Tower.Graph.Nodes;
using UnityEngine;
using XNode;

namespace TDGame.Systems.Tower.Graph
{
    [CreateAssetMenu(menuName = "Data/TowerUpgrade/UpgradeGraph")]
    public class TowerGraph : NodeGraph
    {
        public IEnumerable<TowerDetails> GetHotbarTowers()
        {

            Node hotbarNode = nodes.OfType<HotbarNode>().FirstOrDefault();
            if (!hotbarNode)
                return new List<TowerDetails>();

            var towerNodes = GetConnectedTowerNodes(hotbarNode.GetPort("Next"));
            
            return towerNodes.Select(x => x.TowerDetails);
        }  
        
        public IEnumerable<TowerDetails> GetTowerUpgrades(TowerDetails details)
        {
            var allTowerNodes = nodes.OfType<TowerNode>();

            var enumerable = allTowerNodes.ToList();
            
            var node = enumerable.FirstOrDefault((x) => x.TowerDetails.Name == details.Name);
            if (!node)
                return new TowerDetails[] {};

            var towerNodes = GetConnectedTowerNodes(node.GetPort("Next"));
            
            return towerNodes.Select(x => x.TowerDetails);
        }

        IEnumerable<TowerNode> GetConnectedTowerNodes(NodePort port)
        {
            var connections = port.GetConnections();
            var towerNodes = new TowerNode[connections.Count];

            for (int i = 0; i < connections.Count; i++)
            {
                var connection = connections[i];
                if (connection.node is TowerNode towerNode)
                {
                    towerNodes[i] = towerNode;
                }
            }

            return towerNodes;
        }
        
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
                if (graphNode.TowerDetails.name == towerName)
                {
                    return graphNode;
                }
            }

            return null;
        }
    }
}