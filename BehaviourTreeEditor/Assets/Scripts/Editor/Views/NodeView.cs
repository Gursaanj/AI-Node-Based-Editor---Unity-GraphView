using UnityEngine;

namespace Gbt
{
    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        public Node node;
        
        public NodeView(Node node)
        {
            this.node = node;
            title = node.name;
            viewDataKey = node.guid;

            style.left = node.Position.x;
            style.top = node.Position.y;
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            node.Position = new Vector2(newPos.xMin, newPos.yMin);
        }
    }
}
