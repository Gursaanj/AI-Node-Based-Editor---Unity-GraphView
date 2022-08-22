using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Gbt
{
    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        public Action<NodeView> OnNodeSelected;
        public Node node;

        private const string RUNNING_CLASS_LIST = "running";
        private const string FAILURE_CLASS_LIST = "failure";
        private const string SUCCESS_CLASS_LIST = "success";
        
        private const string DRAG_SELECTION_TYPE = "DragSelection";

        private Port _inputPort;
        private Port _outputPort;

        public Port InputPort => _inputPort;
        public Port OutputPort => _outputPort;

        public NodeView(Node node) : base("Assets/Scripts/Editor/NodeView.uxml")
        {
            this.node = node;
            title = node.name;
            viewDataKey = node.guid;

            style.left = node.position.x;
            style.top = node.position.y;
            
            RegisterCallback<DragUpdatedEvent>(OnDragUpdated, TrickleDown.NoTrickleDown);
            RegisterCallback<DragPerformEvent>(OnDragPerformed, TrickleDown.NoTrickleDown);
            
            CreateInputPorts();
            CreateOutputPorts();
            SetupClasses();

            generateVisualContent = OnVisualContentGenerated;

            //Ensure root node is not deletable or copyable
            if (node is RootNode)
            {
                capabilities -= Capabilities.Deletable;
                capabilities -= Capabilities.Copiable;
            }
            
            //Data bind custom description to Node description
            Label descriptionLabel = this.Q<Label>("description");
            descriptionLabel.bindingPath = nameof(node.description);
            descriptionLabel.Bind(new SerializedObject(node));
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Undo.RecordObject(node, "Set Node Position");
            node.position = new Vector2(newPos.xMin, newPos.yMin);
            EditorUtility.SetDirty(node); //Manually forces recording to persist through events like assembly reloads
        }
        
        public override void OnSelected()
        {
            base.OnSelected();

            OnNodeSelected?.Invoke(this);
        }

        public void UpdateState()
        {
            RemoveFromClassList(RUNNING_CLASS_LIST);
            RemoveFromClassList(FAILURE_CLASS_LIST);
            RemoveFromClassList(SUCCESS_CLASS_LIST);
            
            if (!Application.isPlaying)
            {
                return;
            }

            switch (node.state)
            {
                case Node.State.Running:
                    if (node.hasStarted)
                    {
                        AddToClassList(RUNNING_CLASS_LIST);
                    }
                    break;
                case Node.State.Failure:
                    AddToClassList(FAILURE_CLASS_LIST);
                    break;
                case Node.State.Success:
                    AddToClassList(SUCCESS_CLASS_LIST);
                    break;
            }
        }

        private void OnVisualContentGenerated(MeshGenerationContext ctx)
        {
            VisualElement element = ctx.visualElement.contentContainer.Q("input");
            elementTypeColor = element.resolvedStyle.backgroundColor;
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

        private void OnDragUpdated(DragUpdatedEvent evt)
        {
            if (DragAndDrop.GetGenericData(DRAG_SELECTION_TYPE) is List<ISelectable> selection &&
                (selection.OfType<BlackboardField>().Count() >= 1))
            {
                DragAndDrop.visualMode = evt.actionKey ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Move;
            }
        }

        private void OnDragPerformed(DragPerformEvent evt)
        {
            List<ISelectable> selection = DragAndDrop.GetGenericData(DRAG_SELECTION_TYPE) as List<ISelectable>;
            IEnumerable<BlackboardField> fields = selection.OfType<BlackboardField>();
            
            Undo.RecordObject(node, "Inject Blackboard Data");
            foreach (BlackboardField field in fields)
            {
                node.InjectData(BehaviourTreeView.GetBlackboardFieldData(field));
            }
        }
    }
}
