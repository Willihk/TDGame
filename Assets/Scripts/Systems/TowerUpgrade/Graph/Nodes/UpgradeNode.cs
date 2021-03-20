using UnityEngine;
using XNode;

namespace TDGame.Systems.TowerUpgrade.Graph.Nodes
{
    [CreateNodeMenu("Tower/Upgrade")]
    public class UpgradeNode : Node
    {
        public GameObject TowerPrefab;
        
        [Input(ShowBackingValue.Never)]
        public Node Previous;

        [Output]
        public Node Next;
    }
}