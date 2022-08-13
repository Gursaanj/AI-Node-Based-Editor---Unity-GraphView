using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
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

        public Action<NodeView> OnNodeSelected;
        private BehaviourTree _behaviourTree;
        
        public BehaviourTreeView()
        {
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Editor/BehaviourTreeEditorWindow.uss");
            styleSheets.Add(styleSheet);

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
                tree.rootNode = tree.CreateNode(typeof(RootNode)) as RootNode;
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
            
            EditorApplication.delayCall += () => ResetViewToFitAllContent(true);
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

            foreach (Type actionType in actionTypes)
            {
                evt.menu.AppendAction($"[{actionType.BaseType.Name}] {actionType.Name}", action => CreateNode(actionType));
            }
            
            evt.menu.AppendSeparator();

            foreach (Type compositeType in compositeTypes)
            {
                evt.menu.AppendAction($"[{compositeType.BaseType.Name}] {compositeType.Name}", action => CreateNode(compositeType));
            }
            
            evt.menu.AppendSeparator();

            foreach (Type decoratorType in decoratorTypes)
            {
                evt.menu.AppendAction($"[{decoratorType.BaseType.Name}] {decoratorType.Name}", action => CreateNode(decoratorType));
            }
            
            evt.menu.AppendAction("Reset View", action => ResetViewToFitAllContent(false));
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

        private void CreateNode(System.Type type)
        {
            Node node = _behaviourTree.CreateNode(type);
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

        private void OnUndoRedoPerformed()
        {
            PopulateView(_behaviourTree);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
