using UnityEngine;

namespace Gbt
{
    public class RootNode : Node
    {
        [SerializeField] private Node _child;

        public Node Child
        {
            get => _child;
            set => _child = value;
        }

        public override string InspectorName { get; protected set; } = "Root Node";

        public override Node Clone()
        {
            RootNode node = Instantiate(this);
            node.Child = _child.Clone();
            return node;
        }

        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            return _child.Update();
        }
    }
}
