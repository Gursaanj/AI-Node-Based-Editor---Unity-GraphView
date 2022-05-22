using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gbt
{
    [CreateAssetMenu(menuName = "Gbt/BehaviourTree")]
    public class BehaviourTree : ScriptableObject
    {
        [SerializeField] private Node _rootNode;
        [SerializeField] private Node.State _treeState = Node.State.Running;

        public Node RootNode => _rootNode;
        public Node.State TreeState => _treeState;

        public Node.State Update()
        {
            return _rootNode.Update();
        }
    }
}
