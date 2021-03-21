using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

namespace TDGame.Systems.Tower.Graph.Nodes
{
    [CreateNodeMenu("Tower/Tower")]
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