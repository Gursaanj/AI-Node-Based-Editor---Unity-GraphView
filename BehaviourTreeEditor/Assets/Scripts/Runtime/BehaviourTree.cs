using UnityEngine;

namespace Gbt
{
    [CreateAssetMenu(menuName = "Gbt/BehaviourTree")]
    public class BehaviourTree : ScriptableObject
    {
        public Node rootNode;
        public Node.State treeState;

        public Node.State Update()
        {
            if (rootNode.NodeState == Node.State.Running)
            {
                treeState = rootNode.Update();
            }

            return treeState;
        }
    }
}
