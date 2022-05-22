using System.Collections.Generic;
using UnityEngine;

namespace Gbt
{
    public class BehaviourTreeRunner : MonoBehaviour
    {
        [SerializeField] private BehaviourTree _tree;

        private void Start()
        {
            DebugNode logNode1 = ScriptableObject.CreateInstance<DebugNode>();
            logNode1.type = LogType.Log;
            logNode1.message = "Hello World 1";

            DebugNode logNode2 = ScriptableObject.CreateInstance<DebugNode>();
            logNode2.type = LogType.Log;
            logNode2.message = "Hello World 2";
            
            DebugNode logNode3 = ScriptableObject.CreateInstance<DebugNode>();
            logNode3.type = LogType.Log;
            logNode3.message = "Hello World 3";

            WaitNode waitNode = ScriptableObject.CreateInstance<WaitNode>();
            waitNode.duration = 1.0f;

            SequencerNode sequencerNode = ScriptableObject.CreateInstance<SequencerNode>();
            sequencerNode.Children = new List<Node> {logNode1, waitNode, logNode2};
            
            RepeatNode repeatNode = ScriptableObject.CreateInstance<RepeatNode>();
            repeatNode.numberOfRepetitions = -1;
            repeatNode.ChildNode = sequencerNode;

            _tree.rootNode = repeatNode;
        }

        private void Update()
        {
            _tree.Update();
        }
    }
}
