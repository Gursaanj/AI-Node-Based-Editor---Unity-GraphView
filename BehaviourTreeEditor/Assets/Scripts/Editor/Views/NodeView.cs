using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Gbt
{
    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        public Action<NodeView> OnNodeSelected;
        public Node node;

        private Port _inputPort;
        private Port _outputPort;

        public Port InputPort => _inputPort;
        public Port OutputPort => _outputPort;

        public NodeView(Node node) : base("Assets/Scripts/Editor/NodeView.uxml")
        {
            this.node = node;
            title = node.name;
            viewDataKey = node.Guid;

            style.left = node.position.x;
            style.top = node.position.y;
            
            CreateInputPorts();
            CreateOutputPorts();
            SetupClasses();
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Undo.RecordObject(node, "Node Positional Change" );
            node.position = new Vector2(newPos.xMin, newPos.yMin);
            EditorUtility.SetDirty(node); //helps keep persistent through certain events like assembly reloads
        }

        private void CreateInputPorts()
        {
            switch (node)
            {
                case ActionNode _:
                    _inputPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
                    break;
                case CompositeNode _:
                    _inputPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
                    break;
                case DecoratorNode _:
                    _inputPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
                    break;
                case RootNode _:
                    break;
            }

            if (_inputPort != null)
            {
                _inputPort.portName = string.Empty;
                _inputPort.style.flexDirection = FlexDirection.Column;
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
                    _outputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
                    break;
                case DecoratorNode _:
                    _outputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
                    break;
                case RootNode _:
                    _outputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
                    break;
            }

            if (_outputPort != null)
            {
                _outputPort.portName = string.Empty;
                _outputPort.style.flexDirection = FlexDirection.ColumnReverse;
                outputContainer.Add(_outputPort);
            }
        }

        private void SetupClasses()
        {
            //Tag USS document with node type, to differentiate in UI Builder
            switch (node) 
            {
                case ActionNode _:
                    AddToClassList("action");
                    break;
                case CompositeNode _:
                    AddToClassList("composite");
                    break;
                case DecoratorNode _:
                    AddToClassList("decorator");
                    break;
                case RootNode _:
                    AddToClassList("root");
                    break;
            }
        }

        public override void OnSelected()
        {
            base.OnSelected();

            if (OnNodeSelected != null)
            {
                OnNodeSelected.Invoke(this);
            }
        }
    }
}
