using System.Collections.Generic;

namespace Gbt
{
    public abstract class CompositeNode : Node
    {
       protected List<Node> _children = new List<Node>();

       public List<Node> Children
       {
           get => _children;
           set => _children = value;
       }
    }
}
