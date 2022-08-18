using Sirenix.OdinInspector;
using XNode;

namespace TDGame.Systems.Tower.Graph.Nodes
{
    [CreateNodeMenuAttribute("Tower/Hotbar")]
    public class HotbarNode : Node
    {
        [Output]
        public Node Next;
    }
}