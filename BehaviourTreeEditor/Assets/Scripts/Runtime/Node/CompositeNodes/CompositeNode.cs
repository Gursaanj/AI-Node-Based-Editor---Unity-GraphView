using System.Collections.Generic;

namespace Gbt
{
    public abstract class CompositeNode : Node
    {
        protected List<Node> _children = new List<Node>();
    }
}
