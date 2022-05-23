using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Gbt
{
    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        public Node node;

        private Port _inputPort;
        private Port _outputPort;

        public Port InputPort => _inputPort;
        public Port OutputPort => _outputPort;

        public NodeView(Node node)
        {
            this.node = node;
            title = node.name;
            viewDataKey = node.guid;

            style.left = node.Position.x;
            style.top = node.Position.y;
            
            CreateInputPorts();
            CreateOutputPorts();
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            node.Position = new Vector2(newPos.xMin, newPos.yMin);
        }

        private void CreateInputPorts()
        {
            switch (node)
            {
                case ActionNode _:
                    _inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
                    break;
                case CompositeNode _:
                    _inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
                    break;
                case DecoratorNode _:
                    _inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
                    break;
            }

            if (_inputPort != null)
            {
                _inputPort.portName = string.Empty;
                inputContainer.Add(_inputPort);
            }
        }

        private void CreateOutputPorts()
        {
            switch (node) 
            {
                case ActionNode _:
                    break;
                case CompositeNode _:
                    _outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
                    break;
                case DecoratorNode _:
                    _outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
                    break;
            }

            if (_outputPort != null)
            {
                _outputPort.portName = string.Empty;
                outputContainer.Add(_outputPort);
            }
        }
    }
}
