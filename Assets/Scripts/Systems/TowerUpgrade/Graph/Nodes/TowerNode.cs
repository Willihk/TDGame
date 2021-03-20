using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

namespace TDGame.Systems.TowerUpgrade.Graph.Nodes
{
    [CreateNodeMenu("Tower/Upgrade")]
    public class TowerNode : Node
    {
        [AssetsOnly]
        [AssetSelector(Paths = "Assets/Resources/Prefabs/Towers")]
        [Title("Tower Prefab")]
        [HideLabel]
        public GameObject TowerPrefab;
        
        [HorizontalGroup("Bottom")]
        [Input(ShowBackingValue.Never)]
        public Node Previous;
        
        [HorizontalGroup("Bottom")]
        [Output]
        public Node Next;
    }
}