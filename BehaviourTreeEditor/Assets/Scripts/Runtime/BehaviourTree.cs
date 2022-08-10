using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Gbt
{
    [CreateAssetMenu(menuName = "Gbt/BehaviourTree")]
    public class BehaviourTree : ScriptableObject
    {
        public Node rootNode;
        public Node.State treeState;

        public List<Node> nodes = new List<Node>();

        public Node.State Update()
        {
            if (rootNode.NodeState == Node.State.Running)
            {
                treeState = rootNode.Update();
            }

            return treeState;
        }

        public Node CreateNode(System.Type type)
        {
            Node node = (Node) ScriptableObject.CreateInstance(type);
            node.name = node.InspectorName;
            node.Guid = GUID.Generate().ToString(); //Todo: This is part of UnityEditor, circumvent somehow, maybe as parameter
            nodes.Add(node);
            
#if UNITY_EDITOR
            AssetDatabase.AddObjectToAsset(node, this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif            
            
            return node;
        }

        public void DeleteNode(Node node)
        {
            nodes.Remove(node);
            
#if UNITY_EDITOR
            AssetDatabase.RemoveObjectFromAsset(node);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }

        public void AddChild(Node parent, Node child)
        {
            RootNode root = parent as RootNode;

            if (root != null)
            {
                root.Child = child;
            }

            DecoratorNode decoratorNode = parent as DecoratorNode;

            if (decoratorNode != null)
            {
                decoratorNode.Child = child;
            }
            
            CompositeNode compositeNode = parent as CompositeNode;

            if (compositeNode != null)
            {
                compositeNode.Children.Add(child);
            }
        }

        public void RemoveChild(Node parent, Node child)
        {
            RootNode root = parent as RootNode;

            if (root != null)
            {
                root.Child = null;
            }
            
            DecoratorNode decoratorNode = parent as DecoratorNode;

            if (decoratorNode != null)
            {
                decoratorNode.Child = null;
            }
            
            CompositeNode compositeNode = parent as CompositeNode;

            if (compositeNode != null && compositeNode.Children != null && compositeNode.Children.Count != 0)
            {
                compositeNode.Children.Remove(child);
            }
        }

        public List<Node> GetChildren(Node parent)
        {
            List<Node> children = new List<Node>();
            
            RootNode root = parent as RootNode;

            if (root != null && root.Child != null)
            {
                children.Add(root.Child);
            }
            
            DecoratorNode decoratorNode = parent as DecoratorNode;

            if (decoratorNode != null && decoratorNode.Child != null)
            {
                children.Add(decoratorNode.Child);
            }
            
            CompositeNode compositeNode = parent as CompositeNode;

            if (compositeNode != null && compositeNode.Children != null)
            {
                return compositeNode.Children;
            }

            return children;
        }

        public BehaviourTree Clone()
        {
            BehaviourTree tree = Instantiate(this);
            tree.rootNode = tree.rootNode.Clone();
            return tree;
        }
    }
}
