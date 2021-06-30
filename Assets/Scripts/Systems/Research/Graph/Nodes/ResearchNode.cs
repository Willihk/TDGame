using UnityEngine;
using XNode;

namespace TDGame.Systems.Research.Graph.Nodes
{
    public class ResearchNode : Node
    {
        public GameObject prefab;
        
        
        [Input(ShowBackingValue.Never)]
        public Node Parent;

        [Output]
        public Node Children;

        
        public override object GetValue(NodePort port)
        {
            GetOutputPort("Children");
            return port.fieldName == "input" ? GetInputValue(Parent.name, "None") : null;
        }
    }
}