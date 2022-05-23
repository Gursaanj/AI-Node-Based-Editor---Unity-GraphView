using System;
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
        }

        public void PopulateView(BehaviourTree tree)
        {
            _behaviourTree = tree;

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;

            foreach (Node node in tree.nodes)
            {
                CreateNodeView(node);
            }
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove == null)
            {
                return graphViewChange;
            }
            
            foreach (GraphElement element in graphViewChange.elementsToRemove)
            {
                NodeView nodeView = element as NodeView;

                if (nodeView != null)
                {
                    _behaviourTree.DeleteNode(nodeView.node);
                }
            }

            return graphViewChange;
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
        }

        private void CreateNode(System.Type type)
        {
            Node node = _behaviourTree.CreateNode(type);
            CreateNodeView(node);
        }

        private void CreateNodeView(Node node)
        {
            NodeView nodeView = new NodeView(node);
            AddElement(nodeView);
        }
    }
}
