using Sirenix.OdinInspector;
using TDGame.Systems.Tower.Graph.Data;
using UnityEngine;
using XNode;

namespace TDGame.Systems.Tower.Graph.Nodes
{
    [CreateNodeMenu("Tower/Tower")]
    public class TowerNode : Node
    {
        [AssetsOnly]
        [AssetSelector]
        [Title("Tower Prefab")]
        [HideLabel]
        public TowerDetails TowerDetails;
        
        [HorizontalGroup("Bottom")]
        [Input(ShowBackingValue.Never)]
        public Node Previous;
        
        [HorizontalGroup("Bottom")]
        [Output]
        public Node Next;
    }
}