using UnityEngine;

namespace Gbt
{
    public abstract class DecoratorNode : Node
    {
        [SerializeField] protected Node _childNode;
    }
}
