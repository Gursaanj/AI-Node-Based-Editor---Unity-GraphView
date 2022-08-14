using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
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
        
#if UNITY_EDITOR
        public List<StickyNote> stickyNotes = new List<StickyNote>();
#endif

        public Node.State Update()
        {
            if (rootNode.NodeState == Node.State.Running)
            {
                treeState = rootNode.Update();
            }

            return treeState;
        }
        
#if UNITY_EDITOR
        public Node CreateNode(System.Type type, Vector2 position)
        {
            Node node = (Node) ScriptableObject.CreateInstance(type);
            node.name = node.InspectorName;
            node.position = position;
            node.guid = GUID.Generate().ToString();
            
            Undo.RecordObject(this, "Create Node");
            nodes.Add(node);
            
            AssetDatabase.AddObjectToAsset(node, this);
            Undo.RegisterCreatedObjectUndo(node, "Create Node");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return node;
        }

        public void DeleteNode(Node node)
        {
            Undo.RecordObject(this, "Delete Node");
            nodes.Remove(node);
            
            Undo.DestroyObjectImmediate(node);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public StickyNote CreateStickyNote(Vector2 position)
        {
            StickyNote stickyNote = new StickyNote(position)
            {
                title = "New Note",
                contents = "Add comment here",
                fontSize = StickyNoteFontSize.Small,
                theme = StickyNoteTheme.Classic
            };
            
            stickyNotes.Add(stickyNote);

            return stickyNote;
        }

        public void DeleteStickyNote(StickyNote note)
        {
            stickyNotes.Remove(note);
        }

        public void AddChild(Node parent, Node child)
        {
            RootNode root = parent as RootNode;
            if (root != null)
            {
                Undo.RecordObject(root, "Add Child");
                root.Child = child;
                EditorUtility.SetDirty(root);
            }

            DecoratorNode decoratorNode = parent as DecoratorNode;
            if (decoratorNode != null)
            {
                Undo.RecordObject(decoratorNode, "Add Child");
                decoratorNode.Child = child;
                EditorUtility.SetDirty(decoratorNode);
            }
            
            CompositeNode compositeNode = parent as CompositeNode;
            if (compositeNode != null)
            {
                Undo.RecordObject(compositeNode, "Add Child");
                compositeNode.Children.Add(child);
                EditorUtility.SetDirty(compositeNode);
            }
        }

        public void RemoveChild(Node parent, Node child)
        {
            RootNode root = parent as RootNode;
            if (root != null)
            {
                Undo.RecordObject(root, "Remove Child");
                root.Child = null;
                EditorUtility.SetDirty(root);
            }
            
            DecoratorNode decoratorNode = parent as DecoratorNode;
            if (decoratorNode != null)
            {
                Undo.RecordObject(decoratorNode, "Remove Child");
                decoratorNode.Child = null;
                EditorUtility.SetDirty(decoratorNode);
            }
            
            CompositeNode compositeNode = parent as CompositeNode;
            if (compositeNode != null && compositeNode.Children != null && compositeNode.Children.Count != 0)
            {
                Undo.RecordObject(compositeNode, "Remove Child");
                compositeNode.Children.Remove(child);
                EditorUtility.SetDirty(compositeNode);
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
#endif  
    }
}
