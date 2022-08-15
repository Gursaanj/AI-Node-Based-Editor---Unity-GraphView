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
    public class BehaviourTreeView : GraphView
    {
        //Container class to add BehaviourTreeView as Custom Control in UIBuilder
        public new class UxmlFactory : UxmlFactory<BehaviourTreeView, GraphView.UxmlTraits>
        {
            
        }

        // based on GraphView.k_FrameBorder
        private const int FRAME_BORDER_WIDTH = 30;
        private const string CREATE_NODE_CONTEXT_MENU_PREFIX = "Add Node/";

        public Action<NodeView> OnNodeSelected;
        private BehaviourTree _behaviourTree;
        private Blackboard _blackboard;
        
        public BehaviourTreeView()
        {
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Editor/BehaviourTreeEditorWindow.uss");
            styleSheets.Add(styleSheet);

            GenerateBlackboard();

            Undo.undoRedoPerformed += OnUndoRedoPerformed;
        }

        public void PopulateView(BehaviourTree tree)
        {
            _behaviourTree = tree;
            
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;

            if (tree.rootNode == null)
            {
                tree.rootNode = tree.CreateNode(typeof(RootNode), Vector2.zero) as RootNode;
                EditorUtility.SetDirty(tree);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            

            //Create Node View
            foreach (Node node in tree.nodes)
            {
                CreateNodeView(node);
            }
            
            //Create edges
            foreach (Node node in tree.nodes)
            {
                List<Node> children = tree.GetChildren(node);
                NodeView parentView = FindNodeView(node);
                
                foreach (Node child in children)
                {
                    NodeView childView = FindNodeView(child);

                    Edge edge = parentView.OutputPort.ConnectTo(childView.InputPort);
                    AddElement(edge);
                }
            }
            
            //Create Sticky Notes
            foreach (StickyNote stickyNote in tree.stickyNotes)
            {
                CreateStickyNoteView(stickyNote);
            }
            
            EditorApplication.delayCall += () => ResetViewToFitAllContent(true);
        }

        public override bool supportsWindowedBlackboard => true;

        public override Blackboard GetBlackboard()
        {
            return _blackboard;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(endPort =>
                endPort.direction != startPort.direction &&
                endPort.node != startPort.node).ToList();
        }
        
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            TypeCache.TypeCollection actionTypes = TypeCache.GetTypesDerivedFrom<ActionNode>();
            TypeCache.TypeCollection compositeTypes = TypeCache.GetTypesDerivedFrom<CompositeNode>();
            TypeCache.TypeCollection decoratorTypes = TypeCache.GetTypesDerivedFrom<DecoratorNode>();
            
            //Convert screenSpace to local transform space - With Dragger and Zoomer Manipulators
            //https://answers.unity.com/questions/1825041/how-to-get-the-correct-contextual-menu-mouse-posit.html
            Vector2 localMousePosition = evt.localMousePosition;
            Vector2 graphPosition = viewTransform.matrix.inverse.MultiplyPoint(localMousePosition);

            foreach (Type actionType in actionTypes)
            {
                evt.menu.AppendAction($"{CREATE_NODE_CONTEXT_MENU_PREFIX}{actionType.BaseType.Name}/{actionType.Name}", action => CreateNode(actionType, graphPosition));
            }

            foreach (Type compositeType in compositeTypes)
            {
                evt.menu.AppendAction($"{CREATE_NODE_CONTEXT_MENU_PREFIX}{compositeType.BaseType.Name}/{compositeType.Name}", action => CreateNode(compositeType, graphPosition));
            }

            foreach (Type decoratorType in decoratorTypes)
            {
                evt.menu.AppendAction($"{CREATE_NODE_CONTEXT_MENU_PREFIX}{decoratorType.BaseType.Name}/{decoratorType.Name}", action => CreateNode(decoratorType, graphPosition));
            }
            
            evt.menu.AppendSeparator();
            evt.menu.AppendAction("Add StickyNote", action => CreateStickyNote(graphPosition));
            
            evt.menu.AppendSeparator();
            evt.menu.AppendAction("Reset View", action => ResetViewToFitAllContent(false));
        }

        public void UpdateNodeStates()
        {
            nodes.ForEach(node =>
            {
                NodeView nodeView = node as NodeView;
                nodeView.UpdateState();
            });
        }

        //From https://forum.unity.com/threads/graph-view-transform-that-fits-all-elements.1276886/
        private void ResetViewToFitAllContent(bool maintainCurrentViewScale)
        {
            Rect rectToFit = CalculateRectToFitAll(contentViewContainer);
            CalculateFrameTransform(rectToFit, layout, FRAME_BORDER_WIDTH, out Vector3 frameTranslation, out Vector3 frameScaling);
            Matrix4x4.TRS(frameTranslation, Quaternion.identity, frameScaling);
            UpdateViewTransform(frameTranslation, maintainCurrentViewScale ? viewTransform.scale : frameScaling);
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
                foreach (GraphElement element in graphViewChange.elementsToRemove)
                {
                    if (element is NodeView nodeView)
                    {
                        _behaviourTree.DeleteNode(nodeView.node);
                    }

                    if (element is Edge edge)
                    {
                        NodeView parentView = edge.output.node as NodeView;
                        NodeView childView = edge.input.node as NodeView;
                        _behaviourTree.RemoveChild(parentView.node, childView.node);
                    }

                    if (element is StickyNote stickyNote)
                    {
                        _behaviourTree.DeleteStickyNote(stickyNote);
                    }
                }
            }

            if (graphViewChange.edgesToCreate != null)
            {
                foreach (Edge edge in graphViewChange.edgesToCreate)
                {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;
                    _behaviourTree.AddChild(parentView.node, childView.node);
                }
            }

            return graphViewChange;
        }

        private void CreateStickyNote(Vector2 position)
        {
            StickyNote stickyNote = _behaviourTree.CreateStickyNote(position);
            CreateStickyNoteView(stickyNote);
        }

        private void CreateStickyNoteView(StickyNote note)
        {
            AddElement(note);
        }

        private void CreateNode(System.Type type, Vector2 position)
        {
            Node node = _behaviourTree.CreateNode(type, position);
            CreateNodeView(node);
        }

        private void CreateNodeView(Node node)
        {
            NodeView nodeView = new NodeView(node);
            nodeView.OnNodeSelected = OnNodeSelected;
            AddElement(nodeView);
        }

        private NodeView FindNodeView(Node node)
        {
            return GetNodeByGuid(node.guid) as NodeView;
        }

        #region Blackboard
        
        private enum FieldType
        {
            String,
            Int,
            Float,
            Vector2,
            Vector3,
            Color
        }

        private const string FIELD_VALUE_LABEL = "Value:";

        private void GenerateBlackboard()
        {
            BlackboardSection mainSection = new BlackboardSection
            {
                headerVisible = false,
                canAcceptDrop = selected => true,
            };
            _blackboard = new Blackboard(this)
            {
                title = "Blackboard",
                subTitle = "Global Elements",
                windowed =  true,
                scrollable = true,
                addItemRequested = board =>
                {
                    GenericMenu menu = new GenericMenu();
                    foreach (int value in Enum.GetValues(typeof(FieldType)))
                    {
                        menu.AddItem(new GUIContent(Enum.GetName(typeof(FieldType), value)), false, () => AddRequestedItem(mainSection, (FieldType) value));
                    }
                    menu.ShowAsContext();
                    
                },
                editTextRequested = (board, element, newValue) =>
                {
                    BlackboardField field = (BlackboardField) element;
                    field.text = newValue;
                },
                moveItemRequested = (board, index, element) =>
                {
                    // if (index <= 1 || element.userData == null)
                    // {
                    //     return;
                    // }
                    //
                    // BlackboardField field = (BlackboardField) element;
                    // VisualElement propertyView = (VisualElement) field.userData;
                    //
                    // VisualElement container = new VisualElement();
                    // container.Add(field);
                    // BlackboardRow row = new BlackboardRow(field, propertyView);
                    // container.Add(row);
                    //
                    // mainSection.ElementAt(0).Remove(element.parent);
                }
            };
            
            _blackboard.Add(mainSection);
        }

        private void AddRequestedItem(BlackboardSection mainSection, FieldType fieldType)
        {
            VisualElement fieldContainer = new VisualElement();
            BlackboardField field = GetBlackboardField(fieldType);
            fieldContainer.Add(field);
            
            VisualElement propertyView = GetPropertyField(fieldType);
            field.userData = propertyView;
            
            BlackboardRow row = new BlackboardRow(field, propertyView);
            fieldContainer.Add(row);
            mainSection.Add(fieldContainer);
        }

        private BlackboardField GetBlackboardField(FieldType fieldType)
        {
            switch (fieldType)
            {
                case FieldType.String:
                    return new BlackboardField(null, "String Field", "string");
                case FieldType.Int:
                    return new BlackboardField(null, "Integer Field", "int");
                case FieldType.Float:
                    return new BlackboardField(null, "Float Field", "float");
                case FieldType.Vector2:
                    return new BlackboardField(null, "Vector2 Field", "Vector2");
                case FieldType.Vector3:
                    return new BlackboardField(null, "Vector3 Field", "Vector3");
                case FieldType.Color:
                    return new BlackboardField(null, "Color Field", "Color");
                default:
                    return null;
            }
        }

        private VisualElement GetPropertyField(FieldType fieldType)
        {
            switch (fieldType)
            {
                case FieldType.String:
                    return new TextField(FIELD_VALUE_LABEL)
                    {
                        value = String.Empty
                    };
                case FieldType.Int:
                    return new IntegerField(FIELD_VALUE_LABEL)
                    {
                        value = 0
                    };
                case FieldType.Float:
                    return new FloatField(FIELD_VALUE_LABEL)
                    {
                        value = 0.0f
                    };;
                case FieldType.Vector2:
                    return new Vector2Field(FIELD_VALUE_LABEL)
                    {
                        value = Vector2.zero
                    };
                case FieldType.Vector3:
                    return new Vector3Field(FIELD_VALUE_LABEL)
                    {
                        value = Vector3.zero
                    };
                case FieldType.Color:
                    return new ColorField(FIELD_VALUE_LABEL)
                    {
                        value = Color.black
                    };
                default:
                    return null;
            }
        }

        #endregion

        private void OnUndoRedoPerformed()
        {
            PopulateView(_behaviourTree);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
