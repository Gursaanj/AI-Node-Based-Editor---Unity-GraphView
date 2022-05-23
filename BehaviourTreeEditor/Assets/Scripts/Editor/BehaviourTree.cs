using System.Collections.Generic;
using UnityEditor;
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
            node.Guid = GUID.Generate().ToString();
            nodes.Add(node);

            AssetDatabase.AddObjectToAsset(node, this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return node;
        }

        public void DeleteNode(Node node)
        {
            nodes.Remove(node);
            AssetDatabase.RemoveObjectFromAsset(node);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
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
                decoratorNode.ChildNode = child;
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
                decoratorNode.ChildNode = null;
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

            if (decoratorNode != null && decoratorNode.ChildNode != null)
            {
                children.Add(decoratorNode.ChildNode);
            }
            
            CompositeNode compositeNode = parent as CompositeNode;

            if (compositeNode != null && compositeNode.Children != null)
            {
                return compositeNode.Children;
            }

            return children;
        }
    }
}
