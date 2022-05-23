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
