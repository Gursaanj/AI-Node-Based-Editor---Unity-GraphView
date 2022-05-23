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
            node.guid = GUID.Generate().ToString();
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
    }
}
