using UnityEngine;

namespace Gbt
{
    public abstract class DecoratorNode : Node
    {
        [SerializeField] private Node _child;

        public Node Child
        {
            get => _child;
            set => _child = value;
        }

        public override Node Clone()
        {
            DecoratorNode node = Instantiate(this);
            node.Child = Child.Clone();
            return node;
        }
    }
}
